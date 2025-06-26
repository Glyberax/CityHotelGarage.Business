using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Extensions;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Services;

public class GarageService : IGarageService
{
    private readonly IGarageRepository _garageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public GarageService(IGarageRepository garageRepository, IHotelRepository hotelRepository, IMapper mapper)
    {
        _garageRepository = garageRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<GarageDto>>> GetAllGaragesAsync()
    {
        try
        {
            var garageDtos = await _garageRepository.GetGaragesWithDetails()
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ServiceResult<IEnumerable<GarageDto>>.Success(garageDtos, "Garajlar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<GarageDto>>.Failure($"Garajlar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<GarageDto>> GetGarageByIdAsync(int id)
    {
        try
        {
            var garageDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (garageDto == null)
            {
                return ServiceResult<GarageDto>.Failure("Garaj bulunamadı.");
            }

            return ServiceResult<GarageDto>.Success(garageDto, "Garaj başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<GarageDto>.Failure($"Garaj getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IEnumerable<GarageDto>>> GetGaragesByHotelAsync(int hotelId)
    {
        try
        {
            var garageDtos = await _garageRepository.GetGaragesByHotel(hotelId)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ServiceResult<IEnumerable<GarageDto>>.Success(garageDtos, "Oteldeki garajlar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<GarageDto>>.Failure($"Oteldeki garajlar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<GarageDto>> CreateGarageAsync(GarageCreateDto garageDto)
    {
        try
        {
            // Otel var mı kontrol et
            var hotelExists = await _hotelRepository.ExistsAsync(garageDto.HotelId);
            if (!hotelExists)
            {
                return ServiceResult<GarageDto>.Failure("Belirtilen otel bulunamadı.");
            }

            // AutoMapper ile DTO'yu Entity'e çevir
            var garage = _mapper.Map<Garage>(garageDto);
            var createdGarage = await _garageRepository.AddAsync(garage);

            // AutoMapper projection ile bilgiyi al
            var resultDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == createdGarage.Id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<GarageDto>.Success(resultDto, "Garaj başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return ServiceResult<GarageDto>.Failure($"Garaj oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<GarageDto>> UpdateGarageAsync(int id, GarageCreateDto garageDto)
    {
        try
        {
            var existingGarage = await _garageRepository.GetByIdAsync(id);
            if (existingGarage == null)
            {
                return ServiceResult<GarageDto>.Failure("Güncellenecek garaj bulunamadı.");
            }

            // Otel var mı kontrol et
            var hotelExists = await _hotelRepository.ExistsAsync(garageDto.HotelId);
            if (!hotelExists)
            {
                return ServiceResult<GarageDto>.Failure("Belirtilen otel bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(garageDto, existingGarage);
            await _garageRepository.UpdateAsync(existingGarage);

            // AutoMapper projection ile güncellenmiş veriyi al
            var resultDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<GarageDto>.Success(resultDto, "Garaj başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<GarageDto>.Failure($"Garaj güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteGarageAsync(int id)
    {
        try
        {
            var exists = await _garageRepository.ExistsAsync(id);
            if (!exists)
            {
                return ServiceResult.Failure("Silinecek garaj bulunamadı.");
            }

            var deleted = await _garageRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ServiceResult.Failure("Garaj silinirken hata oluştu.");
            }

            return ServiceResult.Success("Garaj başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Garaj silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<int>> GetAvailableSpacesAsync(int garageId)
    {
        try
        {
            var availableSpaces = await _garageRepository.GetAvailableSpacesAsync(garageId);
            return ServiceResult<int>.Success(availableSpaces, "Müsait alan sayısı başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Failure($"Müsait alan sayısı getirilirken hata oluştu: {ex.Message}");
        }
    }
}