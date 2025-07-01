using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;
using FluentValidation;

namespace CityHotelGarage.Business.Operations.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CarCreateDto> _carCreateValidator;
    private readonly IValidator<CarUpdateDto> _carUpdateValidator;

    public CarService(
        ICarRepository carRepository, 
        IMapper mapper, 
        IValidator<CarCreateDto> carCreateValidator,
        IValidator<CarUpdateDto> carUpdateValidator)
    {
        _carRepository = carRepository;
        _mapper = mapper;
        _carCreateValidator = carCreateValidator;
        _carUpdateValidator = carUpdateValidator;
    }

    public async Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync()
    {
        try
        {
            var carDtos = await _carRepository.GetCarsWithDetails()
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<CarDto>>.Success(carDtos, "Arabalar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CarDto>>.Failure($"Arabalar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CarDto>> GetCarByIdAsync(int id)
    {
        try
        {
            var carDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (carDto == null)
            {
                return Result<CarDto>.Failure("Araba bulunamadı.");
            }

            return Result<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CarDto>> GetCarByLicensePlateAsync(string licensePlate)
    {
        try
        {
            var carDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.LicensePlate == licensePlate)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (carDto == null)
            {
                return Result<CarDto>.Failure("Belirtilen plaka ile araba bulunamadı.");
            }

            return Result<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    // ✅ YENİ: GetCarsByGarageAsync implementasyonu
    public async Task<Result<IEnumerable<CarDto>>> GetCarsByGarageAsync(int garageId)
    {
        try
        {
            var carDtos = await _carRepository.GetCarsWithDetails()
                .Where(c => c.GarageId == garageId)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<CarDto>>.Success(carDtos, "Garajdaki arabalar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CarDto>>.Failure($"Garajdaki arabalar getirilirken hata oluştu: {ex.Message}");
        }
    }

    // ✅ RENAMED: ParkCarAsync → CreateCarAsync
    public async Task<Result<CarDto>> CreateCarAsync(CarCreateDto carDto)
    {
        try
        {
            // FluentValidation ile async validation
            var validationResult = await _carCreateValidator.ValidateAsync(carDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<CarDto>.Failure("Validation hatası", errors);
            }

            // AutoMapper ile DTO'yu Entity'e çevir
            var car = _mapper.Map<Car>(carDto);
            var createdCar = await _carRepository.AddAsync(car);

            // AutoMapper projection ile bilgiyi al
            var resultDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == createdCar.Id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<CarDto>.Success(resultDto, "Araba başarıyla park edildi.");
        }
        catch (Exception ex)
        {
            return Result<CarDto>.Failure($"Araba park edilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<CarDto>> UpdateCarAsync(int id, CarUpdateDto carDto)
    {
        try
        {
            // CarUpdateDto'da ID set et
            carDto.Id = id;

            // FluentValidation ile async validation
            var validationResult = await _carUpdateValidator.ValidateAsync(carDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<CarDto>.Failure("Validation hatası", errors);
            }

            var existingCar = await _carRepository.GetByIdAsync(id);
            if (existingCar == null)
            {
                return Result<CarDto>.Failure("Güncellenecek araba bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(carDto, existingCar);
            await _carRepository.UpdateAsync(existingCar);
            
            // AutoMapper projection
            var resultDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<CarDto>.Success(resultDto, "Araba başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return Result<CarDto>.Failure($"Araba güncellenirken hata oluştu: {ex.Message}");
        }
    }

    // ✅ RENAMED: RemoveCarAsync → DeleteCarAsync  
    public async Task<Result> DeleteCarAsync(int id)
    {
        try
        {
            var exists = await _carRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Failure("Silinecek araba bulunamadı.");
            }

            var deleted = await _carRepository.DeleteAsync(id);
            if (!deleted)
            {
                return Result.Failure("Araba silinirken hata oluştu.");
            }

            return Result.Success("Araba başarıyla park yerinden çıkarıldı.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Araba çıkarılırken hata oluştu: {ex.Message}");
        }
    }

    // 🔄 LEGACY METHODS - Controller uyumluluğu için (geriye dönük uyumluluk)
    public async Task<Result<CarDto>> ParkCarAsync(CarCreateDto carDto)
    {
        return await CreateCarAsync(carDto);
    }

    public async Task<Result> RemoveCarAsync(int id)
    {
        return await DeleteCarAsync(id);
    }
}