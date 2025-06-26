using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IGarageRepository _garageRepository;
    private readonly IMapper _mapper;

    public CarService(ICarRepository carRepository, IGarageRepository garageRepository, IMapper mapper)
    {
        _carRepository = carRepository;
        _garageRepository = garageRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<CarDto>>> GetAllCarsAsync()
    {
        try
        {
            // AutoMapper projection
            var carDtos = await _carRepository.GetCarsWithDetails()
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ServiceResult<IEnumerable<CarDto>>.Success(carDtos, "Arabalar başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<CarDto>>.Failure($"Arabalar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> GetCarByIdAsync(int id)
    {
        try
        {
            var carDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (carDto == null)
            {
                return ServiceResult<CarDto>.Failure("Araba bulunamadı.");
            }

            return ServiceResult<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> GetCarByLicensePlateAsync(string licensePlate)
    {
        try
        {
            var carDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.LicensePlate == licensePlate)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (carDto == null)
            {
                return ServiceResult<CarDto>.Failure("Belirtilen plaka ile araba bulunamadı.");
            }

            return ServiceResult<CarDto>.Success(carDto, "Araba başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> ParkCarAsync(CarCreateDto carDto)
    {
        try
        {
            // Plaka kontrolü
            var existingCar = await _carRepository.IsLicensePlateExistsAsync(carDto.LicensePlate);
            if (existingCar)
            {
                return ServiceResult<CarDto>.Failure("Bu plaka zaten kayıtlı!");
            }

            // Garaj kapasitesi kontrolü
            var availableSpaces = await _garageRepository.GetAvailableSpacesAsync(carDto.GarageId);
            if (availableSpaces <= 0)
            {
                return ServiceResult<CarDto>.Failure("Bu garaj dolu! Başka bir garaj seçin.");
            }

            // AutoMapper ile DTO'yu Entity'e çevir
            var car = _mapper.Map<Car>(carDto);
            var createdCar = await _carRepository.AddAsync(car);
            
            // AutoMapper projection
            var resultDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == createdCar.Id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<CarDto>.Success(resultDto, "Araba başarıyla park edildi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba park edilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CarDto>> UpdateCarAsync(int id, CarCreateDto carDto)
    {
        try
        {
            var existingCar = await _carRepository.GetByIdAsync(id);
            if (existingCar == null)
            {
                return ServiceResult<CarDto>.Failure("Güncellenecek araba bulunamadı.");
            }

            // Plaka kontrolü
            var existingCarWithPlate = await _carRepository.GetCarByLicensePlateAsync(carDto.LicensePlate);
            if (existingCarWithPlate != null && existingCarWithPlate.Id != id)
            {
                return ServiceResult<CarDto>.Failure("Bu plaka başka bir araba tarafından kullanılıyor!");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(carDto, existingCar);
            await _carRepository.UpdateAsync(existingCar);
            
            // AutoMapper projection
            var resultDto = await _carRepository.GetCarsWithDetails()
                .Where(c => c.Id == id)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return ServiceResult<CarDto>.Success(resultDto, "Araba başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CarDto>.Failure($"Araba güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<ServiceResult> RemoveCarAsync(int id)
    {
        try
        {
            var exists = await _carRepository.ExistsAsync(id);
            if (!exists)
            {
                return ServiceResult.Failure("Silinecek araba bulunamadı.");
            }

            var deleted = await _carRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ServiceResult.Failure("Araba silinirken hata oluştu.");
            }

            return ServiceResult.Success("Araba başarıyla park yerinden çıkarıldı.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Araba çıkarılırken hata oluştu: {ex.Message}");
        }
    }
}