using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Operations.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterDtoValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı zorunludur")
            .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir")
            .MustAsync(async (username, cancellation) =>
            {
                return await _userRepository.IsUsernameUniqueAsync(username);
            }).WithMessage("Bu kullanıcı adı zaten kullanılıyor");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
            .MaximumLength(100).WithMessage("E-posta adresi 100 karakterden uzun olamaz")
            .MustAsync(async (email, cancellation) =>
            {
                return await _userRepository.IsEmailUniqueAsync(email);
            }).WithMessage("Bu e-posta adresi zaten kullanılıyor");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Şifre tekrarı zorunludur")
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur")
            .Length(2, 100).WithMessage("Ad 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Ad sadece harf ve boşluk içerebilir");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad zorunludur")
            .Length(2, 100).WithMessage("Soyad 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Soyad sadece harf ve boşluk içerebilir");

        RuleFor(x => x.Role)
            .Must(role => new[] { "User", "Admin", "Manager" }.Contains(role))
            .WithMessage("Geçersiz rol. Geçerli roller: User, Admin, Manager");
    }
}