using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Operations.Validators;

public class HotelUpdateDtoValidator : AbstractValidator<HotelUpdateDto>
{
    private readonly ICityRepository _cityRepository;
    private readonly IHotelRepository _hotelRepository;

    public HotelUpdateDtoValidator(ICityRepository cityRepository, IHotelRepository hotelRepository)
    {
        _cityRepository = cityRepository;
        _hotelRepository = hotelRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir otel ID'si gereklidir")
            .MustAsync(async (id, cancellation) =>
            {
                return await _hotelRepository.ExistsAsync(id);
            }).WithMessage("Güncellenecek otel bulunamadı");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Otel adı zorunludur")
            .Length(3, 150).WithMessage("Otel adı 3-150 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]+$").WithMessage("Otel adı sadece harf, rakam ve boşluk içerebilir");

        RuleFor(x => x.Yildiz)
            .InclusiveBetween(1, 5).WithMessage("Yıldız sayısı 1-5 arasında olmalıdır");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("Geçerli bir şehir seçiniz")
            .MustAsync(async (cityId, cancellation) =>
            {
                return await _cityRepository.ExistsAsync(cityId);
            }).WithMessage("Belirtilen şehir bulunamadı");

        // Aynı şehirde aynı isimde otel olmamalı (kendi ID'si hariç)
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
            {
                return await _hotelRepository.IsHotelNameUniqueInCityAsync(dto.Name, dto.CityId, dto.Id);
            }).WithMessage("Bu şehirde aynı isimde başka bir otel zaten mevcut!");
    }
}