using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityHotelGarage.Business.Repository.Models;

public class Garage
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    public int Capacity { get; set; } 
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    
    // Navigation Property - Bu garajın bağlı olduğu otel (Many-to-One)
    public virtual Hotel Hotel { get; set; } = null!;
    
    // Navigation Property - Bir garajda birden fazla araba olabilir (One-to-Many)
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}