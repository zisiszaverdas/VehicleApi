using VehicleApi.Models;
using Geolocation;
using System.Collections.Concurrent;

namespace VehicleApi.Services;

public class GpsEventGeneratorService : IGpsEventGeneratorService
{
    public const double EventIntervalSeconds = 10.0;
    private readonly Random _random;
    private const double MinLat = 37.831167;
    private const double MinLon = 23.407745;
    private const double MaxLat = 38.205395;
    private const double MaxLon = 24.130096;
    private const double MetersPerSecondToKmPerHour = 3.6;
    private const double MinGeneratedNearbyDistanceMeters = 0;
    private const double MaxGeneratedNearbyDistanceMeters = 50;
    private const double EarthRadiusMeters = 6371000;

    private readonly ConcurrentDictionary<int, (double lat, double lon, DateTime timestamp)> _lastPositions = new();

    public GpsEventGeneratorService() : this(new Random()) { }
    public GpsEventGeneratorService(Random random) => _random = random;

    public void GenerateEvents(IDataStore dataStore)
    {
        foreach (var vehicle in dataStore.Vehicles)
        {
            var now = DateTime.UtcNow;
            if (!_lastPositions.TryGetValue(vehicle.VehicleId, out var prev))
            {
                var (lat, lon) = GetRandomPositionInBoundingBox();
                GenerateInitialPosition(vehicle.VehicleId, lat, lon, now, dataStore);
                continue;
            }
            GenerateNextPosition(vehicle.VehicleId, prev, now, dataStore);
        }
    }

    private void GenerateInitialPosition(int vehicleId, double lat, double lon, DateTime now, IDataStore dataStore)
    {
        _lastPositions[vehicleId] = (lat, lon, now);
        AddEvent(vehicleId, now, 0, lat, lon, dataStore);
    }

    private void GenerateNextPosition(int vehicleId, (double lat, double lon, DateTime timestamp) prev, DateTime now, IDataStore dataStore)
    {
        var (newLat, newLon) = RandomNearbyPosition(prev.lat, prev.lon, MinGeneratedNearbyDistanceMeters, MaxGeneratedNearbyDistanceMeters);
        newLat = Math.Clamp(newLat, MinLat, MaxLat);
        newLon = Math.Clamp(newLon, MinLon, MaxLon);
        var distance = GeoCalculator.GetDistance(prev.lat, prev.lon, newLat, newLon, 1, DistanceUnit.Meters);
        var speed = (distance / EventIntervalSeconds) * MetersPerSecondToKmPerHour;
        AddEvent(vehicleId, now, speed, newLat, newLon, dataStore);
        _lastPositions[vehicleId] = (newLat, newLon, now);
    }

    private (double lat, double lon) RandomNearbyPosition(double lat, double lon, double minMeters, double maxMeters)
    {
        var bearing = _random.NextDouble() * 2 * Math.PI;
        var distance = minMeters + _random.NextDouble() * (maxMeters - minMeters);
        var latRad = DegreesToRadians(lat);
        var lonRad = DegreesToRadians(lon);
        var newLat = Math.Asin(Math.Sin(latRad) * Math.Cos(distance / EarthRadiusMeters) + Math.Cos(latRad) * Math.Sin(distance / EarthRadiusMeters) * Math.Cos(bearing));
        var newLon = lonRad + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / EarthRadiusMeters) * Math.Cos(latRad), Math.Cos(distance / EarthRadiusMeters) - Math.Sin(latRad) * Math.Sin(newLat));
        return (RadiansToDegrees(newLat), RadiansToDegrees(newLon));
    }

    private void AddEvent(int vehicleId, DateTime timestamp, double speed, double lat, double lon, IDataStore dataStore)
    {
        dataStore.Events.Add(new Event
        {
            VehicleId = vehicleId,
            Timestamp = timestamp,
            SpeedKm = speed,
            Latitude = lat,
            Longitude = lon
        });
    }

    private (double lat, double lon) GetRandomPositionInBoundingBox() =>
        (MinLat + _random.NextDouble() * (MaxLat - MinLat), MinLon + _random.NextDouble() * (MaxLon - MinLon));

    private static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    private static double RadiansToDegrees(double rad) => rad * 180.0 / Math.PI;
}


