using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;

namespace CityHotelGarage.Business.Operations.Validators;

public class CityCreateDtoValidator : AbstractValidator<CityCreateDto>
{
    public CityCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Şehir adı zorunludur")
            .Length(2, 100).WithMessage("Şehir adı 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Şehir adı sadece harf ve boşluk içerebilir");

        RuleFor(x => x.Population)
            .GreaterThan(0).WithMessage("Nüfus 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(50000000).WithMessage("Nüfus 50 milyondan fazla olamaz");
    }
}