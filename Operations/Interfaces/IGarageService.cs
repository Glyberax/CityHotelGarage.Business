using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface IGarageService
{
    Task<Result<IEnumerable<GarageDto>>> GetAllGaragesAsync();
    Task<Result<GarageDto>> GetGarageByIdAsync(int id);
    Task<Result<IEnumerable<GarageDto>>> GetGaragesByHotelAsync(int hotelId);
    Task<Result<GarageDto>> CreateGarageAsync(GarageCreateDto garageDto);
    Task<Result<GarageDto>> UpdateGarageAsync(int id, GarageCreateDto garageDto);
    Task<Result> DeleteGarageAsync(int id);
    Task<Result<int>> GetAvailableSpacesAsync(int garageId);
}