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
}