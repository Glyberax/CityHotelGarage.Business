using AutoMapper;
using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityHotelGarage.Business.Operations.Validators;

public class CarCreateDtoValidator : AbstractValidator<CarCreateDto>
{
    private readonly ICarRepository  _carRepository;

    public CarCreateDtoValidator(ICarRepository carRepository)
    {
        _carRepository = carRepository; 
        
        RuleFor(x=>x.OwnerName).MustAsync(async (id,cancell) =>
        {
            return true;
        }).WithMessage("OwnerName is invalid");
        
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Marka alanı zorunludur")
            .Length(2, 50).WithMessage("Marka 2-50 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Marka sadece harf ve boşluk içerebilir");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Plaka alanı zorunludur")
            .Matches(@"^[0-9]{2}[A-ZÇĞıİÖŞÜ]{1,3}[0-9]{2,4}$")
            .WithMessage("Geçerli bir Türk plakası formatı giriniz (örn: 34ABC123)");

        
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Araç sahibi adı zorunludur")
            .Length(2, 100).WithMessage("Araç sahibi adı 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Araç sahibi adı sadece harf ve boşluk içerebilir");

        RuleFor(x => x.GarageId)
            .GreaterThan(0).WithMessage("Geçerli bir garaj seçiniz");
    }
}