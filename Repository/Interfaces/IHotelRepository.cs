using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Interfaces;

public interface IHotelRepository : IBaseRepository<Hotel>
{
    IQueryable<Hotel> GetHotelsWithDetails();
    Task<Hotel?> GetHotelWithDetailsAsync(int id);
    IQueryable<Hotel> GetHotelsByCity(int cityId);
    
    // Async
    Task<bool> IsHotelNameUniqueInCityAsync(string hotelName, int cityId, int? excludeHotelId = null);
}