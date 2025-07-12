using VehicleApi.Models;

namespace VehicleApi.Services;

public class DataLoader
{
    private readonly ILogger<DataLoader> _logger;

    public DataLoader(ILogger<DataLoader> logger)
    {
        _logger = logger;
    }

    public List<Category> LoadCategories(string filePath)
    {
        var categories = new List<Category>();
        try
        {
            foreach (var line in ReadLinesSkipHeader(filePath))
            {
                var parts = line.Split('\t');
                if (parts.Length >= 3 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int speedLimit) && int.TryParse(parts[2], out int duration))
                {
                    categories.Add(new Category
                    {
                        CategoryId = id,
                        SpeedLimitKm = speedLimit,
                        SpeedLimitDurationSeconds = duration
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories from {FilePath}", filePath);
        }
        return categories;
    }

    public List<Vehicle> LoadVehicles(string filePath)
    {
        var vehicles = new List<Vehicle>();
        try
        {
            foreach (var line in ReadLinesSkipHeader(filePath))
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int categoryId))
                {
                    vehicles.Add(new Vehicle
                    {
                        VehicleId = id,
                        CategoryId = categoryId
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load vehicles from {FilePath}", filePath);
        }
        return vehicles;
    }

    public List<Event> LoadEvents(string filePath)
    {
        var events = new List<Event>();
        try
        {
            foreach (var line in ReadLinesSkipHeader(filePath))
            {
                var parts = line.Split('\t');
                if (parts.Length >= 5 && int.TryParse(parts[0], out int vehicleId) && DateTime.TryParse(parts[1], out DateTime timestamp) && double.TryParse(parts[2], out double speed) && double.TryParse(parts[3], out double lat) && double.TryParse(parts[4], out double lng))
                {
                    events.Add(new Event
                    {
                        VehicleId = vehicleId,
                        Timestamp = timestamp,
                        SpeedKm = speed,
                        Latitude = lat,
                        Longitude = lng
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load events from {FilePath}", filePath);
        }
        return events;
    }

    private IEnumerable<string> ReadLinesSkipHeader(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            yield break;
        }
        using var reader = new StreamReader(filePath);
        // Skip header
        reader.ReadLine();
        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine()!;
        }
    }
}
