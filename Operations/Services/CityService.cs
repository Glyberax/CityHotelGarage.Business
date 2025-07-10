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
        // Eski cache'leri temizle
        _cacheService.Remove(ALL_CITIES_KEY);
    
        // Sayfalı cache'leri temizle (pattern ile)
        _cacheService.RemoveByPattern("cities:paged");
    
        _logger.LogInformation("🧹 City cache'leri temizlendi (normal + paged)");
    }
    

/// <summary>
/// Sayfalı şehir listesi getirir - Arama, sıralama ve cache destekli
/// </summary>
public async Task<Result<PagedResult<CityDto>>> GetPagedCitiesAsync(PagingRequestDto pagingRequest)
{
    try
    {
        // Input validation ve default değerler
        if (pagingRequest.PageNumber <= 0) pagingRequest.PageNumber = 1;
        if (pagingRequest.PageSize <= 0 || pagingRequest.PageSize > PagingRequestDto.MaxPageSize)
            pagingRequest.PageSize = 10;

        // Cache key oluştur (tüm parametreleri içeren benzersiz key)
        string cacheKey = $"cities:paged:{pagingRequest.PageNumber}:{pagingRequest.PageSize}:" +
                         $"{pagingRequest.SearchTerm ?? "null"}:{pagingRequest.SortBy ?? "name"}:" +
                         $"{pagingRequest.SortDescending}";

        // 1️⃣ Cache kontrolü
        var cachedResult = _cacheService.Get<PagedResult<CityDto>>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogInformation("✅ Paged cities cache HIT: {CacheKey}", cacheKey);
            return Result<PagedResult<CityDto>>.Success(cachedResult, "Sayfalı şehirler cache'den getirildi.");
        }

        _logger.LogInformation("❌ Paged cities cache MISS: {CacheKey}", cacheKey);

        // 2️⃣ Base query oluştur (Include ile)
        var query = _cityRepository.GetCitiesWithHotels();

        // 3️⃣ Search filtresi uygula (Service layer'da)
        if (!string.IsNullOrWhiteSpace(pagingRequest.SearchTerm))
        {
            var searchTerm = pagingRequest.SearchTerm.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(searchTerm));
            
            _logger.LogInformation("🔍 Search applied: {SearchTerm}", searchTerm);
        }

        // 4️⃣ Sorting uygula (Service layer'da)
        query = pagingRequest.SortBy?.ToLower() switch
        {
            "population" => pagingRequest.SortDescending 
                ? query.OrderByDescending(c => c.Population)
                : query.OrderBy(c => c.Population),
            "createddate" => pagingRequest.SortDescending
                ? query.OrderByDescending(c => c.CreatedDate)
                : query.OrderBy(c => c.CreatedDate),
            _ => pagingRequest.SortDescending  // Default: name
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name)
        };

        _logger.LogInformation("📊 Sorting applied: {SortBy} {Direction}", 
            pagingRequest.SortBy ?? "name", 
            pagingRequest.SortDescending ? "DESC" : "ASC");

        // 5️⃣ Repository'den sayfalı veri al
        var (cities, totalCount) = await _cityRepository.GetPagedAsync(
            query, 
            pagingRequest.PageNumber, 
            pagingRequest.PageSize
        );

        // 6️⃣ DTO'ya çevir
        var cityDtos = _mapper.Map<List<CityDto>>(cities);

        // 7️⃣ PagedResult oluştur
        var pagedResult = new PagedResult<CityDto>(
            cityDtos,
            pagingRequest.PageNumber,
            pagingRequest.PageSize,
            totalCount,
            $"Sayfa {pagingRequest.PageNumber} - {cityDtos.Count} şehir getirildi."
        );

        // 8️⃣ Cache'e kaydet (30 dakika - daha kısa süre çünkü dinamik)
        _cacheService.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(30));

        _logger.LogInformation("📦 Paged cities cached: {TotalRecords} total, {CurrentPage}/{TotalPages} pages",
            totalCount, pagingRequest.PageNumber, pagedResult.Pagination.TotalPages);

        return Result<PagedResult<CityDto>>.Success(pagedResult, pagedResult.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "GetPagedCitiesAsync error - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", 
            pagingRequest.PageNumber, pagingRequest.PageSize, pagingRequest.SearchTerm);
        
        return Result<PagedResult<CityDto>>.Failure($"Sayfalı şehirler getirilirken hata oluştu: {ex.Message}");
    }
}
}
