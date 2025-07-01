using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Operations.Validators;

public class CarUpdateDtoValidator : AbstractValidator<CarUpdateDto>
{
    private readonly ICarRepository _carRepository;
    private readonly IGarageRepository _garageRepository;

    public CarUpdateDtoValidator(ICarRepository carRepository, IGarageRepository garageRepository)
    {
        _carRepository = carRepository;
        _garageRepository = garageRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir araç ID'si gereklidir")
            .MustAsync(async (id, cancellation) =>
            {
                return await _carRepository.ExistsAsync(id);
            }).WithMessage("Güncellenecek araç bulunamadı");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Marka alanı zorunludur")
            .Length(2, 50).WithMessage("Marka 2-50 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]+$").WithMessage("Marka sadece harf, rakam ve boşluk içerebilir");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Plaka alanı zorunludur")
            .Matches(@"^[0-9]{2}[A-ZÇĞIİÖŞÜ]{1,3}[0-9]{2,4}$")
            .WithMessage("Geçerli bir Türk plakası formatı giriniz (örn: 34ABC123)")
            .MustAsync(async (dto, licensePlate, cancellation) =>
            {
                // Update işlemi için - kendi ID'si hariç plaka unique olmalı
                return await _carRepository.IsLicensePlateUniqueAsync(licensePlate, dto.Id);
            }).WithMessage("Bu plaka başka bir araba tarafından kullanılıyor!");

        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Araç sahibi adı zorunludur")
            .Length(2, 100).WithMessage("Araç sahibi adı 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Araç sahibi adı sadece harf ve boşluk içerebilir");

        RuleFor(x => x.GarageId)
            .GreaterThan(0).WithMessage("Geçerli bir garaj seçiniz")
            .MustAsync(async (garageId, cancellation) =>
            {
                return await _garageRepository.ExistsAsync(garageId);
            }).WithMessage("Belirtilen garaj bulunamadı");
    }
}