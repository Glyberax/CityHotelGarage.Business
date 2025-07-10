// CityService.cs - Cache implementasyonu ile

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Extensions;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;
using FluentValidation;

namespace CityHotelGarage.Business.Operations.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IValidator<CityCreateDto> _cityCreateValidator;
    private readonly IValidator<CityUpdateDto> _cityUpdateValidator;
    private readonly ILogger<CityService> _logger;

    // Cache Keys
    private const string ALL_CITIES_KEY = "cities:all";
    private const string CITY_BY_ID_KEY = "cities:id:{0}";

    public CityService(
        ICityRepository cityRepository, 
        IMapper mapper,
        ICacheService cacheService,
        IValidator<CityCreateDto> cityCreateValidator,
        IValidator<CityUpdateDto> cityUpdateValidator,
        ILogger<CityService> logger)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _cityCreateValidator = cityCreateValidator;
        _cityUpdateValidator = cityUpdateValidator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CityDto>>> GetAllCitiesAsync()
    {
        try
        {
            //Cache'den kontrol et
            var cachedCities = _cacheService.Get<List<CityDto>>(ALL_CITIES_KEY);
            if (cachedCities != null)
            {
                return Result<IEnumerable<CityDto>>.Success(cachedCities, "Şehirler cache'den getirildi.");
            }

            //Veritabanından al - mevcut kodunuzdaki metodu kullanıyoruz
            var cityDtos = await _cityRepository.GetCitiesWithHotels()
                .ProjectToCityDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            //Cache'e kaydet (4 saat)
            _cacheService.Set(ALL_CITIES_KEY, cityDtos, TimeSpan.FromHours(4));

            return Result<IEnumerable<CityDto>>.Success(cityDtos, "Şehirler başarıyla getirildi ve cache'lendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllCitiesAsync error");
            return Result<IEnumerable<CityDto>>.Failure($"Şehirler getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CityDto>> GetCityByIdAsync(int id)
    {
        try
        {
            string cacheKey = string.Format(CITY_BY_ID_KEY, id);
            
            //Cache kontrolü
            var cachedCity = _cacheService.Get<CityDto>(cacheKey);
            if (cachedCity != null)
            {
                return Result<CityDto>.Success(cachedCity, "Şehir cache'den getirildi.");
            }

            //Veritabanından al
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return Result<CityDto>.Failure("Şehir bulunamadı.");
            }

            var cityDto = _mapper.Map<CityDto>(city);

            // Cache'e kaydet (2 saat)
            _cacheService.Set(cacheKey, cityDto, TimeSpan.FromHours(2));

            return Result<CityDto>.Success(cityDto, "Şehir başarıyla getirildi ve cache'lendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCityByIdAsync error for ID: {CityId}", id);
            return Result<CityDto>.Failure($"Şehir getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CityDto>> CreateCityAsync(CityCreateDto cityDto)
    {
        try
        {
            // Validation
            var validationResult = await _cityCreateValidator.ValidateAsync(cityDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<CityDto>.Failure("Validation hatası", errors);
            }

            var city = _mapper.Map<City>(cityDto);
            await _cityRepository.AddAsync(city);

            // Cache temizle
            InvalidateCityCaches();

            var resultDto = _mapper.Map<CityDto>(city);
            return Result<CityDto>.Success(resultDto, "Şehir başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCityAsync error");
            return Result<CityDto>.Failure($"Şehir oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CityDto>> UpdateCityAsync(int id, CityUpdateDto cityDto)
    {
        try
        {
            // Validation
            var validationResult = await _cityUpdateValidator.ValidateAsync(cityDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<CityDto>.Failure("Validation hatası", errors);
            }

            var existingCity = await _cityRepository.GetByIdAsync(id);
            if (existingCity == null)
            {
                return Result<CityDto>.Failure("Güncellenecek şehir bulunamadı.");
            }

            _mapper.Map(cityDto, existingCity);
            await _cityRepository.UpdateAsync(existingCity);

            // Cache temizle
            InvalidateCityCaches();
            _cacheService.Remove(string.Format(CITY_BY_ID_KEY, id));

            var resultDto = _mapper.Map<CityDto>(existingCity);
            return Result<CityDto>.Success(resultDto, "Şehir başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateCityAsync error for ID: {CityId}", id);
            return Result<CityDto>.Failure($"Şehir güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCityAsync(int id)
    {
        try
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return Result.Failure("Silinecek şehir bulunamadı.");
            }

            await _cityRepository.DeleteAsync(id);

            // Cache temizle
            InvalidateCityCaches();
            _cacheService.Remove(string.Format(CITY_BY_ID_KEY, id));

            return Result.Success("Şehir başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteCityAsync error for ID: {CityId}", id);
            return Result.Failure($"Şehir silinirken hata oluştu: {ex.Message}");
        }
    }

    private void InvalidateCityCaches()
    {
        _cacheService.Remove(ALL_CITIES_KEY);
        _logger.LogInformation("🧹 City cache'leri temizlendi");
    }
}