// Repository/Interfaces/IUserRepository.cs
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
    Task UpdateLastLoginAsync(int userId);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime);
    Task RevokeRefreshTokenAsync(int userId);
}