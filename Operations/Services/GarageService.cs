using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Extensions;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;
using FluentValidation;

namespace CityHotelGarage.Business.Operations.Services;

public class GarageService : IGarageService
{
    private readonly IGarageRepository _garageRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GarageCreateDto> _garageCreateValidator;
    private readonly IValidator<GarageUpdateDto> _garageUpdateValidator;

    public GarageService(
        IGarageRepository garageRepository, 
        IMapper mapper,
        IValidator<GarageCreateDto> garageCreateValidator,
        IValidator<GarageUpdateDto> garageUpdateValidator)
    {
        _garageRepository = garageRepository;
        _mapper = mapper;
        _garageCreateValidator = garageCreateValidator;
        _garageUpdateValidator = garageUpdateValidator;
    }

    public async Task<Result<IEnumerable<GarageDto>>> GetAllGaragesAsync()
    {
        try
        {
            var garageDtos = await _garageRepository.GetGaragesWithDetails()
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GarageDto>>.Success(garageDtos, "Garajlar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<GarageDto>>.Failure($"Garajlar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<GarageDto>> GetGarageByIdAsync(int id)
    {
        try
        {
            var garageDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (garageDto == null)
            {
                return Result<GarageDto>.Failure("Garaj bulunamadı.");
            }

            return Result<GarageDto>.Success(garageDto, "Garaj başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<GarageDto>.Failure($"Garaj getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<GarageDto>>> GetGaragesByHotelAsync(int hotelId)
    {
        try
        {
            var garageDtos = await _garageRepository.GetGaragesByHotel(hotelId)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GarageDto>>.Success(garageDtos, "Oteldeki garajlar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<GarageDto>>.Failure($"Oteldeki garajlar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<GarageDto>> CreateGarageAsync(GarageCreateDto garageDto)
    {
        try
        {
            // FluentValidation ile async validation
            var validationResult = await _garageCreateValidator.ValidateAsync(garageDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<GarageDto>.Failure("Validation hatası", errors);
            }

            // AutoMapper ile DTO'yu Entity'e çevir
            var garage = _mapper.Map<Garage>(garageDto);
            var createdGarage = await _garageRepository.AddAsync(garage);

            // AutoMapper projection ile bilgiyi al
            var resultDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == createdGarage.Id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<GarageDto>.Success(resultDto, "Garaj başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return Result<GarageDto>.Failure($"Garaj oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<GarageDto>> UpdateGarageAsync(int id, GarageUpdateDto garageDto)
    {
        try
        {
            // GarageUpdateDto'da ID set et
            garageDto.Id = id;

            // FluentValidation ile async validation
            var validationResult = await _garageUpdateValidator.ValidateAsync(garageDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<GarageDto>.Failure("Validation hatası", errors);
            }

            var existingGarage = await _garageRepository.GetByIdAsync(id);
            if (existingGarage == null)
            {
                return Result<GarageDto>.Failure("Güncellenecek garaj bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(garageDto, existingGarage);
            await _garageRepository.UpdateAsync(existingGarage);

            // AutoMapper projection ile güncellenmiş veriyi al
            var resultDto = await _garageRepository.GetGaragesWithDetails()
                .Where(g => g.Id == id)
                .ProjectToGarageDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<GarageDto>.Success(resultDto, "Garaj başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return Result<GarageDto>.Failure($"Garaj güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> DeleteGarageAsync(int id)
    {
        try
        {
            var exists = await _garageRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Failure("Silinecek garaj bulunamadı.");
            }

            var deleted = await _garageRepository.DeleteAsync(id);
            if (!deleted)
            {
                return Result.Failure("Garaj silinirken hata oluştu.");
            }

            return Result.Success("Garaj başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Garaj silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetAvailableSpacesAsync(int garageId)
    {
        try
        {
            var availableSpaces = await _garageRepository.GetAvailableSpacesAsync(garageId);
            return Result<int>.Success(availableSpaces, "Müsait alan sayısı başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Müsait alan sayısı getirilirken hata oluştu: {ex.Message}");
        }
    }
}