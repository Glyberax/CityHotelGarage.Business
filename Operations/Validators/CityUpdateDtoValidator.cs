using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Operations.Validators;

public class CityUpdateDtoValidator : AbstractValidator<CityUpdateDto>
{
    private readonly ICityRepository _cityRepository;

    public CityUpdateDtoValidator(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir şehir ID'si gereklidir")
            .MustAsync(async (id, cancellation) =>
            {
                return await _cityRepository.ExistsAsync(id);
            }).WithMessage("Güncellenecek şehir bulunamadı");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Şehir adı zorunludur")
            .Length(2, 100).WithMessage("Şehir adı 2-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]+$").WithMessage("Şehir adı sadece harf ve boşluk içerebilir")
            .MustAsync(async (dto, cityName, cancellation) =>
            {
                // Update işlemi için - kendi ID'si hariç şehir adı unique olmalı
                return await _cityRepository.IsCityNameUniqueAsync(cityName, dto.Id);
            }).WithMessage("Bu şehir adı başka bir şehir tarafından kullanılıyor!");

        RuleFor(x => x.Population)
            .GreaterThan(0).WithMessage("Nüfus 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(50000000).WithMessage("Nüfus 50 milyondan fazla olamaz");
    }
}