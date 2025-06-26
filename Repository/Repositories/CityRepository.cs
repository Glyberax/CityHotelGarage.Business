using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Repositories;

public class CityRepository : BaseRepository<City>, ICityRepository
{
    public CityRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<City> GetCitiesWithHotels()
    {
        return _context.Cities
            .Include(c => c.Hotels);
    }

    public async Task<City?> GetCityWithHotelsAsync(int id)
    {
        return await GetCitiesWithHotels()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}