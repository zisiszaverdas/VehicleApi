using VehicleApi.Models;
using Geolocation;

namespace VehicleApi.Extensions;

public static class EventExtensions
{
    public static double CalculateTripDistance(this IEnumerable<Event> events)
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
