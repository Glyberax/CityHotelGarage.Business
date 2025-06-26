using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface IHotelService
{
    Task<ServiceResult<IEnumerable<HotelDto>>> GetAllHotelsAsync();
    Task<ServiceResult<HotelDto>> GetHotelByIdAsync(int id);
    Task<ServiceResult<IEnumerable<HotelDto>>> GetHotelsByCityAsync(int cityId);
    Task<ServiceResult<HotelDto>> CreateHotelAsync(HotelCreateDto hotelDto);
    Task<ServiceResult<HotelDto>> UpdateHotelAsync(int id, HotelCreateDto hotelDto);
    Task<ServiceResult> DeleteHotelAsync(int id);
}