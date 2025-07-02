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

    // Async
    public async Task<bool> IsHotelNameUniqueInCityAsync(string hotelName, int cityId, int? excludeHotelId = null)
    {
        var query = _context.Hotels.Where(h => 
            h.Name.ToLower() == hotelName.ToLower() && 
            h.CityId == cityId);
        
        if (excludeHotelId.HasValue)
        {
            query = query.Where(h => h.Id != excludeHotelId.Value);
        }
        
        return !await query.AnyAsync();
    }
}