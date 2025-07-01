using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface IHotelService
{
    Task<Result<IEnumerable<HotelDto>>> GetAllHotelsAsync();
    Task<Result<HotelDto>> GetHotelByIdAsync(int id);
    Task<Result<IEnumerable<HotelDto>>> GetHotelsByCityAsync(int cityId);
    Task<Result<HotelDto>> CreateHotelAsync(HotelCreateDto hotelDto);
    Task<Result<HotelDto>> UpdateHotelAsync(int id, HotelCreateDto hotelDto);
    Task<Result> DeleteHotelAsync(int id);
}