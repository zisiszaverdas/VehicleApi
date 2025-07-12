using VehicleApi.DTOs;
using VehicleApi.Models;
using Geolocation;

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
        result.TripDistance = GetDistance(events);
        result.Violations = GetViolations(vehicleCategory.Category, events);
        return result;
    }

    private static List<RouteViolationDto> GetViolations(Category category, List<Event> events)
    {
        var violations = new List<RouteViolationDto>();
        if (category != null)
        {
            DateTime? violationStart = null;
            foreach (var ev in events)
            {
                if (ev.SpeedKm > category.SpeedLimitKm)
                {
                    if (violationStart == null)
                        violationStart = ev.Timestamp;
                    if ((ev.Timestamp - violationStart.Value).TotalSeconds >= category.SpeedLimitDurationSeconds)
                    {
                        violations.Add(new RouteViolationDto { Timestamp = violationStart.Value, Duration = (ev.Timestamp - violationStart.Value).TotalSeconds });
                        violationStart = null;
                    }
                }
                else
                {
                    violationStart = null;
                }
            }
        }

        return violations;
    }

    private static double GetDistance(List<Event> events)
    {
        double distance = 0;
        Event? prev = null;
        foreach (var ev in events)
        {
            if (prev != null)
            {
                distance += GeoCalculator.GetDistance(prev.Latitude, prev.Longitude, ev.Latitude, ev.Longitude, 1, DistanceUnit.Meters);
            }
            prev = ev;
        }

        return distance;
    }
}
