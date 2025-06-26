using FluentValidation;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Validators;

public class GarageValidator : AbstractValidator<Garage>
{
    public GarageValidator()
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

        RuleFor(x => x.CreatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Oluşturulma tarihi gelecekte olamaz");
    }
}
