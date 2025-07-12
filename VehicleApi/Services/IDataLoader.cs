using VehicleApi.Models;

namespace VehicleApi.Services;

public interface IDataLoader
{
    List<Category> LoadCategories(string filePath);
    List<Vehicle> LoadVehicles(string filePath);
    List<Event> LoadEvents(string filePath);
}
