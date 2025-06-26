using AutoMapper;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // City Mappings
        CreateMap<City, CityDto>()
            .ForMember(dest => dest.HotelCount, opt => opt.MapFrom(src => src.Hotels.Count));
        
        CreateMap<CityCreateDto, City>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Hotels, opt => opt.Ignore());

        // Hotel Mappings
        CreateMap<Hotel, HotelDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.Name : ""))
            .ForMember(dest => dest.GarageCount, opt => opt.MapFrom(src => src.Garages.Count));
        
        CreateMap<HotelCreateDto, Hotel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.City, opt => opt.Ignore())
            .ForMember(dest => dest.Garages, opt => opt.Ignore());

        // Garage Mappings
        CreateMap<Garage, GarageDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : ""))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Hotel != null && src.Hotel.City != null ? src.Hotel.City.Name : ""))
            .ForMember(dest => dest.CarCount, opt => opt.MapFrom(src => src.Cars.Count))
            .ForMember(dest => dest.AvailableSpaces, opt => opt.MapFrom(src => src.Capacity - src.Cars.Count));
        
        CreateMap<GarageCreateDto, Garage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Hotel, opt => opt.Ignore())
            .ForMember(dest => dest.Cars, opt => opt.Ignore());

        // Car Mappings
        CreateMap<Car, CarDto>()
            .ForMember(dest => dest.GarageName, opt => opt.MapFrom(src => src.Garage != null ? src.Garage.Name : ""))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Garage != null && src.Garage.Hotel != null ? src.Garage.Hotel.Name : ""))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Garage != null && src.Garage.Hotel != null && src.Garage.Hotel.City != null ? src.Garage.Hotel.City.Name : ""));
        
        CreateMap<CarCreateDto, Car>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EntryTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Garage, opt => opt.Ignore());
    }
}