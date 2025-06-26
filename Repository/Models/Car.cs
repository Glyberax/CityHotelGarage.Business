using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityHotelGarage.Business.Repository.Models;

public class Car
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Brand { get; set; } = "";
    
    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = "";
    
    [MaxLength(100)]
    public string OwnerName { get; set; } = "";
    
    public DateTime EntryTime { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("Garage")]
    public int GarageId { get; set; }
    
    public virtual Garage? Garage { get; set; }
}