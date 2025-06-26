namespace CityHotelGarage.Business.Operations.DTOs;

public class GarageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public DateTime CreatedDate { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = "";
    public string CityName { get; set; } = "";
    public int CarCount { get; set; }
    public int AvailableSpaces { get; set; }
}

public class GarageCreateDto
{
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public int HotelId { get; set; }
}