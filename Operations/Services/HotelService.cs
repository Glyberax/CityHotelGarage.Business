// HotelService.cs - Cache implementasyonu ile

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

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IValidator<HotelCreateDto> _hotelCreateValidator;
    private readonly IValidator<HotelUpdateDto> _hotelUpdateValidator;
    private readonly ILogger<HotelService> _logger;

    // Cache Keys
    private const string ALL_HOTELS_KEY = "hotels:all";
    private const string HOTEL_BY_ID_KEY = "hotels:id:{0}";
    private const string HOTELS_BY_CITY_KEY = "hotels:city:{0}";

    public HotelService(
        IHotelRepository hotelRepository, 
        IMapper mapper,
        ICacheService cacheService,
        IValidator<HotelCreateDto> hotelCreateValidator,
        IValidator<HotelUpdateDto> hotelUpdateValidator,
        ILogger<HotelService> logger)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _hotelCreateValidator = hotelCreateValidator;
        _hotelUpdateValidator = hotelUpdateValidator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<HotelDto>>> GetAllHotelsAsync()
    {
        try
        {
            // Cache'den kontrol et
            var cachedHotels = _cacheService.Get<List<HotelDto>>(ALL_HOTELS_KEY);
            if (cachedHotels != null)
            {
                return Result<IEnumerable<HotelDto>>.Success(cachedHotels, "Oteller cache'den getirildi.");
            }

            var hotelDtos = await _hotelRepository.GetHotelsWithDetails()
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            // Cache'e kaydet (45 dakika)
            _cacheService.Set(ALL_HOTELS_KEY, hotelDtos, TimeSpan.FromMinutes(45));

            return Result<IEnumerable<HotelDto>>.Success(hotelDtos, "Oteller başarıyla getirildi ve cache'lendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllHotelsAsync error");
            return Result<IEnumerable<HotelDto>>.Failure($"Oteller getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> GetHotelByIdAsync(int id)
    {
        try
        {
            string cacheKey = string.Format(HOTEL_BY_ID_KEY, id);
            
            // Cache kontrolü
            var cachedHotel = _cacheService.Get<HotelDto>(cacheKey);
            if (cachedHotel != null)
            {
                return Result<HotelDto>.Success(cachedHotel, "Otel cache'den getirildi.");
            }

            // Veritabanından al
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
            {
                return Result<HotelDto>.Failure("Otel bulunamadı.");
            }

            var hotelDto = _mapper.Map<HotelDto>(hotel);

            // Cache'e kaydet (1 saat)
            _cacheService.Set(cacheKey, hotelDto, TimeSpan.FromHours(1));

            return Result<HotelDto>.Success(hotelDto, "Otel başarıyla getirildi ve cache'lendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetHotelByIdAsync error for ID: {HotelId}", id);
            return Result<HotelDto>.Failure($"Otel getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<HotelDto>>> GetHotelsByCityAsync(int cityId)
    {
        try
        {
            string cacheKey = string.Format(HOTELS_BY_CITY_KEY, cityId);
            
            // Cache kontrolü
            var cachedHotels = _cacheService.Get<List<HotelDto>>(cacheKey);
            if (cachedHotels != null)
            {
                return Result<IEnumerable<HotelDto>>.Success(cachedHotels, "Şehirdeki oteller cache'den getirildi.");
            }

            // Veritabanından al
            var hotels = await _hotelRepository.GetHotelsByCity(cityId).ToListAsync();
            var hotelDtos = _mapper.Map<List<HotelDto>>(hotels);

            // Cache'e kaydet (30 dakika)
            _cacheService.Set(cacheKey, hotelDtos, TimeSpan.FromMinutes(30));

            return Result<IEnumerable<HotelDto>>.Success(hotelDtos, "Şehirdeki oteller başarıyla getirildi ve cache'lendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetHotelsByCityAsync error for CityId: {CityId}", cityId);
            return Result<IEnumerable<HotelDto>>.Failure($"Şehirdeki oteller getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> CreateHotelAsync(HotelCreateDto hotelDto)
    {
        try
        {
            // Validation
            var validationResult = await _hotelCreateValidator.ValidateAsync(hotelDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<HotelDto>.Failure("Validation hatası", errors);
            }

            var hotel = _mapper.Map<Hotel>(hotelDto);
            await _hotelRepository.AddAsync(hotel);

            // Cache temizle
            InvalidateHotelCaches(hotel.CityId);

            var resultDto = _mapper.Map<HotelDto>(hotel);
            return Result<HotelDto>.Success(resultDto, "Otel başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateHotelAsync error");
            return Result<HotelDto>.Failure($"Otel oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> UpdateHotelAsync(int id, HotelUpdateDto hotelDto)
    {
        try
        {
            // Validation
            var validationResult = await _hotelUpdateValidator.ValidateAsync(hotelDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<HotelDto>.Failure("Validation hatası", errors);
            }

            var existingHotel = await _hotelRepository.GetByIdAsync(id);
            if (existingHotel == null)
            {
                return Result<HotelDto>.Failure("Güncellenecek otel bulunamadı.");
            }

            _mapper.Map(hotelDto, existingHotel);
            await _hotelRepository.UpdateAsync(existingHotel);

            // Cache temizle
            InvalidateHotelCaches(existingHotel.CityId);
            _cacheService.Remove(string.Format(HOTEL_BY_ID_KEY, id));

            var resultDto = _mapper.Map<HotelDto>(existingHotel);
            return Result<HotelDto>.Success(resultDto, "Otel başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateHotelAsync error for ID: {HotelId}", id);
            return Result<HotelDto>.Failure($"Otel güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null)
            {
                return Result.Failure("Silinecek otel bulunamadı.");
            }

            await _hotelRepository.DeleteAsync(id);

            // Cache temizle
            InvalidateHotelCaches(hotel.CityId);
            _cacheService.Remove(string.Format(HOTEL_BY_ID_KEY, id));

            return Result.Success("Otel başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteHotelAsync error for ID: {HotelId}", id);
            return Result.Failure($"Otel silinirken hata oluştu: {ex.Message}");
        }
    }

    private void InvalidateHotelCaches(int cityId)
    {
        _cacheService.Remove(ALL_HOTELS_KEY);
        _cacheService.Remove(string.Format(HOTELS_BY_CITY_KEY, cityId));
        _logger.LogInformation("🧹 Hotel cache'leri temizlendi (CityId: {CityId})", cityId);
    }
}