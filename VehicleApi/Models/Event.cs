namespace VehicleApi.Models;

public class Event
{
    public int VehicleId { get; set; }
    public DateTime Timestamp { get; set; }
    public double SpeedKm { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
