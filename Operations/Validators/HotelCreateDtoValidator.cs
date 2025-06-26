using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;

namespace CityHotelGarage.Business.Operations.Validators;

public class HotelCreateDtoValidator : AbstractValidator<HotelCreateDto>
{
    public HotelCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Otel adı zorunludur")
            .Length(3, 150).WithMessage("Otel adı 3-150 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]+$").WithMessage("Otel adı sadece harf, rakam ve boşluk içerebilir");

        RuleFor(x => x.Yildiz)
            .InclusiveBetween(1, 5).WithMessage("Yıldız sayısı 1-5 arasında olmalıdır");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("Geçerli bir şehir seçiniz");
    }
}