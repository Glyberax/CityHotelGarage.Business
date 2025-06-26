using AutoMapper;
using AutoMapper.QueryableExtensions;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Extensions;

public static class AutoMapperExtensions
{ 
    
    public static IQueryable<CarDto> ProjectToCarDto(this IQueryable<Car> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<CarDto>(configuration);
    }

    public static IQueryable<CityDto> ProjectToCityDto(this IQueryable<City> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<CityDto>(configuration);
    }

    public static IQueryable<HotelDto> ProjectToHotelDto(this IQueryable<Hotel> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<HotelDto>(configuration);
    }

    public static IQueryable<GarageDto> ProjectToGarageDto(this IQueryable<Garage> query, AutoMapper.IConfigurationProvider configuration)
    {
        return query.ProjectTo<GarageDto>(configuration);
    }
}