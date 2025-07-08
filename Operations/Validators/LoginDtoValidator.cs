// Operations/Validators/LoginDtoValidator.cs
using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;

namespace CityHotelGarage.Business.Operations.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı zorunludur")
            .Length(3, 50).WithMessage("Geçerli bir kullanıcı adı giriniz");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur")
            .MinimumLength(6).WithMessage("Geçerli bir şifre giriniz");
    }
}