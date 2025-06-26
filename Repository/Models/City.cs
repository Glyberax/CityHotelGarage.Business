
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CityHotelGarage.Business.Repository.Models;
public class City
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    public int Population { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}