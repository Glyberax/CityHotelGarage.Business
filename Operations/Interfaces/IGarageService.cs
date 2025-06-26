using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface IGarageService
{
    Task<ServiceResult<IEnumerable<GarageDto>>> GetAllGaragesAsync();
    Task<ServiceResult<GarageDto>> GetGarageByIdAsync(int id);
    Task<ServiceResult<IEnumerable<GarageDto>>> GetGaragesByHotelAsync(int hotelId);
    Task<ServiceResult<GarageDto>> CreateGarageAsync(GarageCreateDto garageDto);
    Task<ServiceResult<GarageDto>> UpdateGarageAsync(int id, GarageCreateDto garageDto);
    Task<ServiceResult> DeleteGarageAsync(int id);
    Task<ServiceResult<int>> GetAvailableSpacesAsync(int garageId);
}