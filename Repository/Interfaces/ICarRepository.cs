using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Interfaces;

public interface ICarRepository : IBaseRepository<Car>
{
    IQueryable<Car> GetCarsWithDetails();
    Task<Car?> GetCarWithDetailsAsync(int id);
    Task<Car?> GetCarByLicensePlateAsync(string licensePlate);
    IQueryable<Car> GetCarsByGarage(int garageId);
    Task<bool> IsLicensePlateExistsAsync(string licensePlate);
    
    Task<bool> IsLicensePlateUniqueAsync(string licensePlate, int? excludeCarId = null);
}