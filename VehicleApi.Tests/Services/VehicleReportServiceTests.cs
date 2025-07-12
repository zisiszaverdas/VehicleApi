using NSubstitute;
using VehicleApi.Models;
using VehicleApi.Services;

namespace VehicleApi.Tests.Services;

public class VehicleReportServiceTests
{
    private readonly IDataStore _dataStore;
    private readonly VehicleReportService _service;

    public VehicleReportServiceTests()
    {
        _dataStore = Substitute.For<IDataStore>();
        _service = new VehicleReportService(_dataStore);
    }

    [Fact]
    public void GetRouteByVehicle_ReturnsEmpty_WhenVehicleNotFound()
    {
        _dataStore.Vehicles.Returns(new List<Vehicle>());
        _dataStore.Categories.Returns(new List<Category>());
        _dataStore.Events.Returns(new List<Event>());

        var result = _service.GetRouteByVehicle(1, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
        Assert.Equal(1, result.VehicleId);
        Assert.Empty(result.Positions);
        Assert.Empty(result.Violations);
        Assert.Equal(0, result.TripDistance);
    }

    [Fact]
    public void GetRouteByVehicle_ReturnsEmpty_WhenNoEvents()
    {
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 2 } });
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 2, SpeedLimitKm = 50, SpeedLimitDurationSeconds = 10 } });
        _dataStore.Events.Returns(new List<Event>());

        var result = _service.GetRouteByVehicle(1, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
        Assert.Equal(1, result.VehicleId);
        Assert.Empty(result.Positions);
        Assert.Empty(result.Violations);
        Assert.Equal(0, result.TripDistance);
    }

    [Fact]
    public void GetRouteByVehicle_ReturnsPositions_AndTripDistance()
    {
        var now = DateTime.UtcNow;
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 2 } });
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 2, SpeedLimitKm = 50, SpeedLimitDurationSeconds = 10 } });
        _dataStore.Events.Returns(new List<Event>
        {
            new Event { VehicleId = 1, Timestamp = now, Latitude = 0, Longitude = 0, SpeedKm = 10 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(10), Latitude = 0, Longitude = 0.001, SpeedKm = 20 }
        });

        var result = _service.GetRouteByVehicle(1, now, now.AddMinutes(1));
        Assert.Equal(1, result.VehicleId);
        Assert.Equal(2, result.Positions.Count());
        Assert.True(result.TripDistance > 0);
        Assert.Empty(result.Violations);
    }

    [Fact]
    public void GetRouteByVehicle_DetectsSpeedViolations()
    {
        var now = DateTime.UtcNow;
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 2 } });
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 2, SpeedLimitKm = 50, SpeedLimitDurationSeconds = 10 } });
        _dataStore.Events.Returns(new List<Event>
        {
            new Event { VehicleId = 1, Timestamp = now, Latitude = 0, Longitude = 0, SpeedKm = 60 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(5), Latitude = 0, Longitude = 0.001, SpeedKm = 60 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(15), Latitude = 0, Longitude = 0.002, SpeedKm = 60 }
        });

        var result = _service.GetRouteByVehicle(1, now, now.AddMinutes(1));
        Assert.Single(result.Violations);
        var violation = result.Violations.First();
        Assert.Equal(now, violation.Timestamp);
        Assert.True(violation.Duration >= 10);
    }
}
