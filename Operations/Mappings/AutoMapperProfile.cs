using AutoMapper;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // City → CityDto (Read)
        CreateMap<City, CityDto>()
            .ForMember(dest => dest.HotelCount, opt => opt.MapFrom(src => src.Hotels.Count));
        
        // CityCreateDto → City (Create)
        CreateMap<CityCreateDto, City>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Hotels, opt => opt.Ignore());

        // ✅ YENİ: CityUpdateDto → City (Update)
        CreateMap<CityUpdateDto, City>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // CreatedDate değişmesin
            .ForMember(dest => dest.Hotels, opt => opt.Ignore()); // Navigation property ignore
        
        // Hotel → HotelDto (Read)
        CreateMap<Hotel, HotelDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.Name : ""))
            .ForMember(dest => dest.GarageCount, opt => opt.MapFrom(src => src.Garages.Count));
        
        // HotelCreateDto → Hotel (Create)
        CreateMap<HotelCreateDto, Hotel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.City, opt => opt.Ignore())
            .ForMember(dest => dest.Garages, opt => opt.Ignore());

        CreateMap<HotelUpdateDto, Hotel>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.City, opt => opt.Ignore())
            .ForMember(dest => dest.Garages, opt => opt.Ignore());
        
        // Garage → GarageDto (Read)
        CreateMap<Garage, GarageDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Hotel != null && src.Hotel.City != null ? src.Hotel.City.Name : ""))
            .ForMember(dest => dest.CarCount, opt => opt.MapFrom(src => src.Cars.Count))
            .ForMember(dest => dest.AvailableSpaces, opt => opt.MapFrom(src => src.Capacity - src.Cars.Count));
        
        // GarageCreateDto → Garage (Create)
        CreateMap<GarageCreateDto, Garage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Hotel, opt => opt.Ignore())
            .ForMember(dest => dest.Cars, opt => opt.Ignore());

        CreateMap<GarageUpdateDto, Garage>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.Hotel, opt => opt.Ignore())
            .ForMember(dest => dest.Cars, opt => opt.Ignore());
        
        // Car → CarDto (Read)
        CreateMap<Car, CarDto>()
            .ForMember(dest => dest.GarageName, opt => opt.MapFrom(src => src.Garage != null ? src.Garage.Name : ""))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Garage != null && src.Garage.Hotel != null ? src.Garage.Hotel.Name : ""))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Garage != null && src.Garage.Hotel != null && src.Garage.Hotel.City != null ? src.Garage.Hotel.City.Name : ""));
        
        CreateMap<CarCreateDto, Car>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EntryTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Garage, opt => opt.Ignore());

        CreateMap<CarUpdateDto, Car>()
            .ForMember(dest => dest.EntryTime, opt => opt.Ignore()) // EntryTime değişmesin
            .ForMember(dest => dest.Garage, opt => opt.Ignore()); // Navigation property ignore
        
        CreateMap<City, CityUpdateDto>();
        CreateMap<Hotel, HotelUpdateDto>();
        CreateMap<Garage, GarageUpdateDto>();
        CreateMap<Car, CarUpdateDto>();
    }
}