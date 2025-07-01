using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface ICityService
{
    Task<Result<IEnumerable<CityDto>>> GetAllCitiesAsync();
    Task<Result<CityDto>> GetCityByIdAsync(int id);
    Task<Result<CityDto>> CreateCityAsync(CityCreateDto cityDto);
    Task<Result<CityDto>> UpdateCityAsync(int id, CityCreateDto cityDto);
    Task<Result> DeleteCityAsync(int id);
}