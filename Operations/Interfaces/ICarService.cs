using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface ICarService
{
    Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync();
    Task<Result<CarDto>> GetCarByIdAsync(int id);
    Task<Result<CarDto>> GetCarByLicensePlateAsync(string licensePlate);
    Task<Result<IEnumerable<CarDto>>> GetCarsByGarageAsync(int garageId); 
    Task<Result<CarDto>> CreateCarAsync(CarCreateDto carDto); 
    Task<Result<CarDto>> UpdateCarAsync(int id, CarUpdateDto carDto);
    Task<Result> DeleteCarAsync(int id);
}