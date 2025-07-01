using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Operations.Validators;

public class GarageUpdateDtoValidator : AbstractValidator<GarageUpdateDto>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IGarageRepository _garageRepository;

    public GarageUpdateDtoValidator(IHotelRepository hotelRepository, IGarageRepository garageRepository)
    {
        _hotelRepository = hotelRepository;
        _garageRepository = garageRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir garaj ID'si gereklidir")
            .MustAsync(async (id, cancellation) =>
            {
                return await _garageRepository.ExistsAsync(id);
            }).WithMessage("Güncellenecek garaj bulunamadı");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Garaj adı zorunludur")
            .Length(3, 100).WithMessage("Garaj adı 3-100 karakter arasında olmalıdır")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]+$").WithMessage("Garaj adı sadece harf, rakam ve boşluk içerebilir");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(1000).WithMessage("Kapasite 1000'den fazla olamaz");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("Geçerli bir otel seçiniz")
            .MustAsync(async (hotelId, cancellation) =>
            {
                return await _hotelRepository.ExistsAsync(hotelId);
            }).WithMessage("Belirtilen otel bulunamadı");

        // Aynı otelde aynı isimde garaj olmamalı (kendi ID'si hariç)
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
            {
                return await _garageRepository.IsGarageNameUniqueInHotelAsync(dto.Name, dto.HotelId, dto.Id);
            }).WithMessage("Bu otelde aynı isimde başka bir garaj zaten mevcut!");
    }
}