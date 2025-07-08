// Operations/Validators/ChangePasswordDtoValidator.cs
using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;

namespace CityHotelGarage.Business.Operations.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifre zorunludur");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre zorunludur")
            .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Yeni şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir")
            .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre mevcut şifreden farklı olmalıdır");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Yeni şifre tekrarı zorunludur")
            .Equal(x => x.NewPassword).WithMessage("Yeni şifreler eşleşmiyor");
    }
}