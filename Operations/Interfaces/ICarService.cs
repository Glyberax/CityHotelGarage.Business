using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface ICarService
{
    Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync();
    Task<Result<CarDto>> GetCarByIdAsync(int id);
    Task<Result<CarDto>> GetCarByLicensePlateAsync(string licensePlate);
    Task<Result<CarDto>> ParkCarAsync(CarCreateDto carDto);
    Task<Result<CarDto>> UpdateCarAsync(int id, CarUpdateDto carDto); // ← Değişti!
    Task<Result> RemoveCarAsync(int id); // ← Task<Result> olacak
}