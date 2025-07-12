using System.Collections.Concurrent;
using NSubstitute;
using VehicleApi.Models;
using VehicleApi.Services;
using Xunit;

namespace VehicleApi.Tests.Services;

public class GpsEventGeneratorServiceTests
{
    [Fact]
    public void GenerateEvents_AddsInitialEventForVehicle()
    {
        // Arrange
        var vehicles = new ConcurrentBag<Vehicle> { new Vehicle { VehicleId = 1, CategoryId = 1 } };
        var events = new ConcurrentBag<Event>();
        var dataStore = Substitute.For<IDataStore>();
        dataStore.Vehicles.Returns(vehicles);
        dataStore.Events.Returns(events);
        var service = new GpsEventGeneratorService();

        // Act
        service.GenerateEvents(dataStore);

        // Assert
        Assert.Single(events);
        var ev = events.First();
        Assert.Equal(1, ev.VehicleId);
        Assert.InRange(ev.Latitude, 37.831167, 38.205395);
        Assert.InRange(ev.Longitude, 23.407745, 24.130096);
        Assert.Equal(0, ev.SpeedKm);
    }

    [Fact]
    public void GenerateEvents_AddsNextEventWithSpeed()
    {
        // Arrange
        var vehicles = new ConcurrentBag<Vehicle> { new Vehicle { VehicleId = 2, CategoryId = 1 } };
        var events = new ConcurrentBag<Event>();
        var dataStore = Substitute.For<IDataStore>();
        dataStore.Vehicles.Returns(vehicles);
        dataStore.Events.Returns(events);
        var service = new GpsEventGeneratorService();

        // Act
        service.GenerateEvents(dataStore); // Initial event
        service.GenerateEvents(dataStore); // Next event

        // Assert
        Assert.Equal(2, events.Count);
        var evs = events.ToList();
        Assert.Equal(2, evs[0].VehicleId);
        Assert.Equal(2, evs[1].VehicleId);
        Assert.True(evs[1].SpeedKm >= 0);
        Assert.InRange(evs[1].Latitude, 37.831167, 38.205395);
        Assert.InRange(evs[1].Longitude, 23.407745, 24.130096);
    }

    [Fact]
    public void GenerateEvents_MultipleVehicles_AllGetEvents()
    {
        // Arrange
        var vehicles = new ConcurrentBag<Vehicle> {
            new Vehicle { VehicleId = 1, CategoryId = 1 },
            new Vehicle { VehicleId = 2, CategoryId = 1 }
        };
        var events = new ConcurrentBag<Event>();
        var dataStore = Substitute.For<IDataStore>();
        dataStore.Vehicles.Returns(vehicles);
        dataStore.Events.Returns(events);
        var service = new GpsEventGeneratorService();

        // Act
        service.GenerateEvents(dataStore);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.Contains(events, e => e.VehicleId == 1);
        Assert.Contains(events, e => e.VehicleId == 2);
    }
}
