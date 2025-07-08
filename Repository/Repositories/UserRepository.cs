// Repository/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Data;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                                     u.RefreshTokenExpiryTime > DateTime.UtcNow);
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.Username.ToLower() == username.ToLower());
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        
        return !await query.AnyAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        
        return !await query.AnyAsync();
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            await UpdateAsync(user);
        }
    }

    public async Task RevokeRefreshTokenAsync(int userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await UpdateAsync(user);
        }
    }
}