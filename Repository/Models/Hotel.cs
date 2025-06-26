
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityHotelGarage.Business.Repository.Models;

public class Hotel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = "";
    
    public int Yildiz { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("City")]
    public int CityId { get; set; }
    
    // Navigation Property - Bu otelin bağlı olduğu şehir (Many-to-One)
    public virtual City City { get; set; } = null!;
    
    // Navigation Property - Bir otelin birden fazla garajı olabilir (One-to-Many)
    public virtual ICollection<Garage> Garages { get; set; } = new List<Garage>();
}