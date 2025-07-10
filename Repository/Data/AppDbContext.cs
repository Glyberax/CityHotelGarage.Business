// Repository/Data/AppDbContext.cs (UPDATED)
using Microsoft.EntityFrameworkCore;
using CityHotelGarage.Business.Repository.Models;

namespace CityHotelGarage.Business.Repository.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Garage> Garages { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // City -> Hotel (One-to-Many)
        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.City)
            .WithMany(c => c.Hotels)
            .HasForeignKey(h => h.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Hotel -> Garage (One-to-Many)
        modelBuilder.Entity<Garage>()
            .HasOne(g => g.Hotel)
            .WithMany(h => h.Garages)
            .HasForeignKey(g => g.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Garage -> Car (One-to-Many)
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Garage)
            .WithMany(g => g.Cars)
            .HasForeignKey(c => c.GarageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Car>()
            .HasIndex(c => c.LicensePlate)
            .IsUnique();

        // ✅ YENİ - User Entity Configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User default değerler
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasDefaultValue("User");
            
        modelBuilder.Entity<User>()
            .Property(u => u.IsActive)
            .HasDefaultValue(true);
            
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}