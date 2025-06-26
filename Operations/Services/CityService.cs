using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Extensions;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public CityService(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<CityDto>>> GetAllCitiesAsync()
    {
        try
        {
            var cityDtos = await _cityRepository.GetCitiesWithHotels()
                .ProjectToCityDto(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ServiceResult<IEnumerable<CityDto>>.Success(cityDtos, "Şehirler başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<CityDto>>.Failure($"Şehirler getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> GetCityByIdAsync(int id)
    {
        try
        {
            var cityDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == id)
                .ProjectToCityDto(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (cityDto == null)
            {
                return ServiceResult<CityDto>.Failure("Şehir bulunamadı.");
            }

            return ServiceResult<CityDto>.Success(cityDto, "Şehir başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> CreateCityAsync(CityCreateDto cityDto)
    {
        try
        {
            // AutoMapper ile DTO'yu Entity'e çevir
            var city = _mapper.Map<City>(cityDto);
            var createdCity = await _cityRepository.AddAsync(city);

            // AutoMapper projection ile bilgiyi al
            var resultDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == createdCity.Id)
                .ProjectToCityDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<CityDto>.Success(resultDto, "Şehir başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CityDto>> UpdateCityAsync(int id, CityCreateDto cityDto)
    {
        try
        {
            var existingCity = await _cityRepository.GetByIdAsync(id);
            if (existingCity == null)
            {
                return ServiceResult<CityDto>.Failure("Güncellenecek şehir bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(cityDto, existingCity);
            await _cityRepository.UpdateAsync(existingCity);

            // AutoMapper projection ile güncellenmiş veriyi al
            var resultDto = await _cityRepository.GetCitiesWithHotels()
                .Where(c => c.Id == id)
                .ProjectToCityDto(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<CityDto>.Success(resultDto, "Şehir başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CityDto>.Failure($"Şehir güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteCityAsync(int id)
    {
        try
        {
            var exists = await _cityRepository.ExistsAsync(id);
            if (!exists)
            {
                return ServiceResult.Failure("Silinecek şehir bulunamadı.");
            }

            var deleted = await _cityRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ServiceResult.Failure("Şehir silinirken hata oluştu.");
            }

            return ServiceResult.Success("Şehir başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Şehir silinirken hata oluştu: {ex.Message}");
        }
    }
}