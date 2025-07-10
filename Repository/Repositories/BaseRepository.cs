
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Interfaces;

namespace CityHotelGarage.Business.Repository.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
    
    public virtual async Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(IQueryable<T> query, int pageNumber, int pageSize)
    {
        // Input validation
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max limit

        try
        {
            var totalCount = await query.CountAsync();

            //Sayfalama uygula
            var data = await query
                .Skip((pageNumber - 1) * pageSize)  // İlk N kaydı atla
                .Take(pageSize)                     // Sonraki X kaydı al
                .ToListAsync();

            return (data, totalCount);
        }
        catch (Exception)
        {
            // Hata durumunda boş liste döndür
            return ([], 0);
        }
    }
}