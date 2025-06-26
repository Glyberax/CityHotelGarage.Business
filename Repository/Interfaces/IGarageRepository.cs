using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Interfaces;

public interface IGarageRepository : IBaseRepository<Garage>
{
    IQueryable<Garage> GetGaragesWithDetails();
    Task<Garage?> GetGarageWithDetailsAsync(int id);
    IQueryable<Garage> GetGaragesByHotel(int hotelId);
    Task<int> GetAvailableSpacesAsync(int garageId);
}