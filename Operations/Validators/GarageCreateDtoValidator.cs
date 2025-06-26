using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;

namespace CityHotelGarage.Business.Operations.Validators;

public class GarageCreateDtoValidator : AbstractValidator<GarageCreateDto>
{
    public GarageCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Garaj adı zorunludur")
            .Length(3, 100).WithMessage("Garaj adı 3-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]+$").WithMessage("Garaj adı sadece harf, rakam ve boşluk içerebilir");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(1000).WithMessage("Kapasite 1000'den fazla olamaz");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Geçerli bir otel seçiniz");
    }
}