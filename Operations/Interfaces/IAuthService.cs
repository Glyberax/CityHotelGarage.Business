// Operations/Interfaces/IAuthService.cs
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<Result> LogoutAsync(int userId);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<Result<UserDto>> GetUserProfileAsync(int userId);
}

