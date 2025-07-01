using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Repositories;

public class GarageRepository : BaseRepository<Garage>, IGarageRepository
{
    public GarageRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Garage> GetGaragesWithDetails()
    {
        return _context.Garages
            .Include(g => g.Hotel)
            .ThenInclude(h => h.City)
            .Include(g => g.Cars);
    }

    public async Task<Garage?> GetGarageWithDetailsAsync(int id)
    {
        return await GetGaragesWithDetails()
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public IQueryable<Garage> GetGaragesByHotel(int hotelId)
    {
        return GetGaragesWithDetails()
            .Where(g => g.HotelId == hotelId);
    }

    public async Task<int> GetAvailableSpacesAsync(int garageId)
    {
        var garage = await _context.Garages
            .Include(g => g.Cars)
            .FirstOrDefaultAsync(g => g.Id == garageId);
        
        if (garage == null) return 0;
        
        return garage.Capacity - garage.Cars.Count;
    }

    // Async validation için yeni metodlar
    public async Task<bool> HasAvailableSpaceAsync(int garageId)
    {
        var availableSpaces = await GetAvailableSpacesAsync(garageId);
        return availableSpaces > 0;
    }

    public async Task<bool> IsGarageNameUniqueInHotelAsync(string garageName, int hotelId, int? excludeGarageId = null)
    {
        var query = _context.Garages.Where(g => 
            g.Name.ToLower() == garageName.ToLower() && 
            g.HotelId == hotelId);
        
        if (excludeGarageId.HasValue)
        {
            query = query.Where(g => g.Id != excludeGarageId.Value);
        }
        
        return !await query.AnyAsync();
    }
}