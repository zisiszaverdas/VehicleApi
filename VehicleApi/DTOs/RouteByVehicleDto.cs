namespace VehicleApi.DTOs;

public class RouteByVehicleDto
{
    public int VehicleId { get; set; }
    public IEnumerable<RoutePositionDto> Positions { get; set; } = new List<RoutePositionDto>();
    public double TripDistance { get; set; }
    public IEnumerable<RouteViolationDto> Violations { get; set; } = new List<RouteViolationDto>();
}

public class RoutePositionDto
{
    public DateTime Timestamp { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double SpeedKm { get; set; }
}

public class RouteViolationDto
{
    public DateTime Timestamp { get; set; }
    public double Duration { get; set; }
}
