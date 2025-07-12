using VehicleApi.Models;

namespace VehicleApi.Services;

public class DataStore : IDataStore
{
    public List<Category> Categories { get; set; } = new();
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<Event> Events { get; set; } = new();
}
