using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Extensions;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepository, ICityRepository cityRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HotelDto>>> GetAllHotelsAsync()
    {
        try
        {
            var hotelDtos = await _hotelRepository.GetHotelsWithDetails()
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<HotelDto>>.Success(hotelDtos, "Oteller başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HotelDto>>.Failure($"Oteller getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> GetHotelByIdAsync(int id)
    {
        try
        {
            var hotelDto = await _hotelRepository.GetHotelsWithDetails()
                .Where(h => h.Id == id)
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (hotelDto == null)
            {
                return Result<HotelDto>.Failure("Otel bulunamadı.");
            }

            return Result<HotelDto>.Success(hotelDto, "Otel başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<HotelDto>.Failure($"Otel getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<HotelDto>>> GetHotelsByCityAsync(int cityId)
    {
        try
        {
            var hotelDtos = await _hotelRepository.GetHotelsByCity(cityId)
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<HotelDto>>.Success(hotelDtos, "Şehirdeki oteller başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HotelDto>>.Failure($"Şehirdeki oteller getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> CreateHotelAsync(HotelCreateDto hotelDto)
    {
        try
        {
            // Şehir var mı kontrol et
            var cityExists = await _cityRepository.ExistsAsync(hotelDto.CityId);
            if (!cityExists)
            {
                return Result<HotelDto>.Failure("Belirtilen şehir bulunamadı.");
            }

            // AutoMapper ile DTO'yu Entity'e çevir
            var hotel = _mapper.Map<Hotel>(hotelDto);
            var createdHotel = await _hotelRepository.AddAsync(hotel);

            // AutoMapper projection ile bilgiyi al
            var resultDto = await _hotelRepository.GetHotelsWithDetails()
                .Where(h => h.Id == createdHotel.Id)
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<HotelDto>.Success(resultDto, "Otel başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return Result<HotelDto>.Failure($"Otel oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<HotelDto>> UpdateHotelAsync(int id, HotelCreateDto hotelDto)
    {
        try
        {
            var existingHotel = await _hotelRepository.GetByIdAsync(id);
            if (existingHotel == null)
            {
                return Result<HotelDto>.Failure("Güncellenecek otel bulunamadı.");
            }

            // Şehir var mı kontrol et
            var cityExists = await _cityRepository.ExistsAsync(hotelDto.CityId);
            if (!cityExists)
            {
                return Result<HotelDto>.Failure("Belirtilen şehir bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(hotelDto, existingHotel);
            await _hotelRepository.UpdateAsync(existingHotel);

            // AutoMapper projection ile güncellenmiş veriyi al
            var resultDto = await _hotelRepository.GetHotelsWithDetails()
                .Where(h => h.Id == id)
                .ProjectToHotelDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<HotelDto>.Success(resultDto, "Otel başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return Result<HotelDto>.Failure($"Otel güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        try
        {
            var exists = await _hotelRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Failure("Silinecek otel bulunamadı.");
            }

            var deleted = await _hotelRepository.DeleteAsync(id);
            if (!deleted)
            {
                return Result.Failure("Otel silinirken hata oluştu.");
            }

            return Result.Success("Otel başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Otel silinirken hata oluştu: {ex.Message}");
        }
    }
}