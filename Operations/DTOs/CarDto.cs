namespace CityHotelGarage.Business.Operations.DTOs;

public class CarDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public DateTime EntryTime { get; set; }
    public string GarageName { get; set; } = "";
    public string HotelName { get; set; } = "";
    public string CityName { get; set; } = "";
    public int GarageId { get; set; }
}

public class CarCreateDto
{
    public string Brand { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public int GarageId { get; set; }
}