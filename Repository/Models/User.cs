using System.ComponentModel.DataAnnotations;

namespace CityHotelGarage.Business.Repository.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = "";
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = "";
    
    [Required]
    public string PasswordHash { get; set; } = "";
    
    [MaxLength(100)]
    public string FirstName { get; set; } = "";
    
    [MaxLength(100)]
    public string LastName { get; set; } = "";
    
    [MaxLength(20)]
    public string Role { get; set; } = "User"; // User, Admin, Manager
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginDate { get; set; }
    
    [MaxLength(255)]
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }
}