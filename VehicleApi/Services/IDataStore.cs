using VehicleApi.Models;
using System.Collections.Concurrent;

namespace VehicleApi.Services;

public interface IDataStore
{
    ConcurrentBag<Category> Categories { get; set; }
    ConcurrentBag<Vehicle> Vehicles { get; set; }
    ConcurrentBag<Event> Events { get; set; }
}