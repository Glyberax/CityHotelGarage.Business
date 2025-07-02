using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Interfaces;

public interface ICityRepository : IBaseRepository<City>
{
    IQueryable<City> GetCitiesWithHotels();
    Task<City?> GetCityWithHotelsAsync(int id);
    
    // Async 
    Task<bool> IsCityNameUniqueAsync(string cityName, int? excludeCityId = null);
}