using NSubstitute;
using VehicleApi.Models;
using VehicleApi.Services;

namespace VehicleApi.Tests.Services;

public class CategoryReportServiceTests
{
    private readonly IDataStore _dataStore;
    private readonly CategoryReportService _service;

    public CategoryReportServiceTests()
    {
        _dataStore = Substitute.For<IDataStore>();
        _service = new CategoryReportService(_dataStore);
    }

    [Fact]
    public void GetViolations_ReturnsEmpty_WhenCategoryNotFound()
    {
        _dataStore.Categories.Returns(new List<Category>());
        var result = _service.GetViolations(1, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
        Assert.Empty(result);
    }

    [Fact]
    public void GetViolations_ReturnsEmpty_WhenNoViolations()
    {
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 1, SpeedLimitKm = 50, SpeedLimitDurationSeconds = 10 } });
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 1 } });
        _dataStore.Events.Returns(new List<Event> {
            new Event { VehicleId = 1, Timestamp = DateTime.UtcNow, SpeedKm = 40, Latitude = 0, Longitude = 0 }
        });
        var result = _service.GetViolations(1, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1));
        Assert.Empty(result);
    }

    [Fact]
    public void GetViolations_ReturnsCorrectCount()
    {
        var now = DateTime.UtcNow;
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 1, SpeedLimitKm = 50, SpeedLimitDurationSeconds = 10 } });
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 1 } });
        _dataStore.Events.Returns(new List<Event> {
            new Event { VehicleId = 1, Timestamp = now, SpeedKm = 60, Latitude = 0, Longitude = 0 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(5), SpeedKm = 60, Latitude = 0, Longitude = 0 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(15), SpeedKm = 60, Latitude = 0, Longitude = 0 }
        });
        var result = _service.GetViolations(1, now, now.AddMinutes(1)).ToList();
        Assert.Single(result);
        Assert.Equal(1, result[0].VehicleId);
        Assert.Equal(1, result[0].CountViolations);
    }

    [Fact]
    public void GetTripDistances_ReturnsEmpty_WhenNoEvents()
    {
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 1 } });
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 1 } });
        _dataStore.Events.Returns(new List<Event>());
        var result = _service.GetTripDistances(1, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
        Assert.Empty(result);
    }

    [Fact]
    public void GetTripDistances_ReturnsCorrectDistance()
    {
        var now = DateTime.UtcNow;
        _dataStore.Categories.Returns(new List<Category> { new Category { CategoryId = 1 } });
        _dataStore.Vehicles.Returns(new List<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 1 } });
        _dataStore.Events.Returns(new List<Event> {
            new Event { VehicleId = 1, Timestamp = now, Latitude = 0, Longitude = 0, SpeedKm = 10 },
            new Event { VehicleId = 1, Timestamp = now.AddSeconds(10), Latitude = 0, Longitude = 0.001, SpeedKm = 20 }
        });
        var result = _service.GetTripDistances(1, now, now.AddMinutes(1)).ToList();
        Assert.Single(result);
        Assert.Equal(1, result[0].VehicleId);
        Assert.True(result[0].Distance > 0);
    }
}
