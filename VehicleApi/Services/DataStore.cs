using VehicleApi.Models;
using System.Collections.Concurrent;

namespace VehicleApi.Services;

public class DataStore : IDataStore
{
    public ConcurrentBag<Category> Categories { get; set; } = new();
    public ConcurrentBag<Vehicle> Vehicles { get; set; } = new();
    public ConcurrentBag<Event> Events { get; set; } = new();
}
