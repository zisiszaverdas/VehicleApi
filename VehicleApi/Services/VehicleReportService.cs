using VehicleApi.DTOs;
using VehicleApi.Extensions;

namespace VehicleApi.Services;

public class VehicleReportService(IDataStore dataStore) : IVehicleReportService
{

    public RouteByVehicleDto GetRouteByVehicle(int vehicleId, DateTime fromTime, DateTime toTime)
    {
        var result = new RouteByVehicleDto
        {
            VehicleId = vehicleId,
            Positions = [],
            TripDistance = 0,
            Violations = []
        };

        var vehicleCategory = dataStore.Vehicles
            .Where(v => v.VehicleId == vehicleId)
            .Join(dataStore.Categories,
                  v => v.CategoryId,
                  c => c.CategoryId,
                  (v, c) => new { Vehicle = v, Category = c })
            .FirstOrDefault();
        if (vehicleCategory == null)
            return result;

        var events = dataStore.Events
            .Where(e => e.VehicleId == vehicleId && e.Timestamp >= fromTime && e.Timestamp <= toTime)
            .OrderBy(e => e.Timestamp)
            .ToList();
        if (!events.Any())
            return result;

        result.Positions = events.Select(e => new RoutePositionDto { Timestamp = e.Timestamp, Latitude = e.Latitude, Longitude = e.Longitude, SpeedKm = e.SpeedKm });
        result.TripDistance = events.CalculateTripDistance();
        result.Violations = vehicleCategory.Category.GetSpeedViolations(events,
            (start, duration) => new RouteViolationDto { Timestamp = start, Duration = duration }
        );
        return result;
    }
}
