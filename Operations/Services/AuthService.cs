// Operations/Services/AuthService.cs
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Interfaces;
using CityHotelGarage.Business.Operations.Results;
using CityHotelGarage.Business.Repository.Interfaces;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Operations.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public async Task<Result<LoginResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Validation
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<LoginResponseDto>.Failure("Kayıt bilgilerinde hata var", errors);
            }

            // User oluştur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = registerDto.Role,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // Şifreyi hash'le
            user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

            // Veritabanına kaydet
            var createdUser = await _userRepository.AddAsync(user);

            // Bearer Token oluştur (JWT)
            var accessToken = _jwtService.GenerateAccessToken(createdUser);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Refresh token'ı kaydet
            await _userRepository.UpdateRefreshTokenAsync(createdUser.Id, refreshToken, DateTime.UtcNow.AddDays(30));

            // Response oluştur
            var response = new LoginResponseDto
            {
                AccessToken = accessToken, // Bu Bearer token olarak kullanılacak
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // 1 haftalık token
                User = _mapper.Map<UserDto>(createdUser)
            };

            return Result<LoginResponseDto>.Success(response, "Kullanıcı başarıyla kaydedildi. Bearer token oluşturuldu.");
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure($"Kayıt işlemi sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Validation
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<LoginResponseDto>.Failure("Giriş bilgilerinde hata var", errors);
            }

            // Kullanıcıyı bul
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return Result<LoginResponseDto>.Failure("Kullanıcı adı veya şifre hatalı");
            }

            // Aktif mi kontrol et
            if (!user.IsActive)
            {
                return Result<LoginResponseDto>.Failure("Hesabınız deaktif durumda");
            }

            // Şifre kontrolü
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Result<LoginResponseDto>.Failure("Kullanıcı adı veya şifre hatalı");
            }

            // Bearer Token oluştur (JWT) - 1 HAFTALIK
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Refresh token'ı kaydet ve son giriş tarihini güncelle
            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));
            await _userRepository.UpdateLastLoginAsync(user.Id);

            // Response oluştur
            var response = new LoginResponseDto
            {
                AccessToken = accessToken, // Bu Bearer token olarak kullanılacak
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // 1 haftalık token
                User = _mapper.Map<UserDto>(user)
            };

            return Result<LoginResponseDto>.Success(response, "Giriş başarılı. Bearer token oluşturuldu.");
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure($"Giriş işlemi sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        try
        {
            // Access token'dan principal'ı al
            var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            if (principal == null)
            {
                return Result<LoginResponseDto>.Failure("Geçersiz access token");
            }

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Result<LoginResponseDto>.Failure("Geçersiz token");
            }

            // Kullanıcıyı bul
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result<LoginResponseDto>.Failure("Geçersiz refresh token");
            }

            // Yeni Bearer Token oluştur (JWT)
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Yeni refresh token'ı kaydet
            await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(30));

            // Response oluştur
            var response = new LoginResponseDto
            {
                AccessToken = newAccessToken, // Yeni Bearer token
                RefreshToken = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // 1 haftalık token
                User = _mapper.Map<UserDto>(user)
            };

            return Result<LoginResponseDto>.Success(response, "Bearer token yenilendi");
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure($"Token yenileme sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> LogoutAsync(int userId)
    {
        try
        {
            await _userRepository.RevokeRefreshTokenAsync(userId);
            return Result.Success("Çıkış başarılı. Bearer token iptal edildi.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Çıkış işlemi sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            // Validation
            var validationResult = await _changePasswordValidator.ValidateAsync(changePasswordDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure("Şifre değişikliği bilgilerinde hata var", errors);
            }

            // Kullanıcıyı bul
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("Kullanıcı bulunamadı");
            }

            // Mevcut şifreyi kontrol et
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.CurrentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Result.Failure("Mevcut şifre hatalı");
            }

            // Yeni şifreyi hash'le
            user.PasswordHash = _passwordHasher.HashPassword(user, changePasswordDto.NewPassword);

            // Güncelle
            await _userRepository.UpdateAsync(user);

            // Güvenlik için refresh token'ı iptal et
            await _userRepository.RevokeRefreshTokenAsync(userId);

            return Result.Success("Şifre başarıyla değiştirildi. Güvenlik için tekrar giriş yapmanız gerekiyor.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Şifre değişikliği sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<UserDto>> GetUserProfileAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<UserDto>.Failure("Kullanıcı bulunamadı");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto, "Kullanıcı profili getirildi");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Profil getirilirken hata oluştu: {ex.Message}");
        }
    }
}