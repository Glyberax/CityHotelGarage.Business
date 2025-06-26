using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Repositories;

public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Hotel> GetHotelsWithDetails()
    {
        return _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Garages);
    }

    public async Task<Hotel?> GetHotelWithDetailsAsync(int id)
    {
        return await GetHotelsWithDetails()
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public IQueryable<Hotel> GetHotelsByCity(int cityId)
    {
        return GetHotelsWithDetails()
            .Where(h => h.CityId == cityId);
    }
}