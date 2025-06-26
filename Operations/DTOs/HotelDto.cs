namespace CityHotelGarage.Business.Operations.DTOs;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Yildiz { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = "";
    public int GarageCount { get; set; }
}

public class HotelCreateDto
{
    public string Name { get; set; } = "";
    public int Yildiz { get; set; }
    public int CityId { get; set; }
}