using VehicleApi.Models;

namespace VehicleApi.Services;

public interface IDataStore
{
    List<Category> Categories { get; set; }
    List<Vehicle> Vehicles { get; set; }
    List<Event> Events { get; set; }
}