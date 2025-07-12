using VehicleApi.DTOs;
using VehicleApi.Models;
using VehicleApi.Extensions;

namespace VehicleApi.Services;

public class CategoryReportService(IDataStore dataStore) : ICategoryReportService
{
    public IEnumerable<ViolationDto> GetViolations(int categoryId, DateTime fromTime, DateTime toTime)
    {
        var category = dataStore.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
        if (category == null)
            return Enumerable.Empty<ViolationDto>();

        return GetGroupedEventsByVehicle(categoryId, fromTime, toTime)
            .Select(group => new ViolationDto
            {
                VehicleId = group.Key,
                CountViolations = category.GetSpeedViolations(group, (start, duration) => 1).Count()
            })
            .Where(dto => dto.CountViolations > 0)
            .OrderByDescending(dto => dto.CountViolations)
            .ToList();
    }

    public IEnumerable<TripDistanceDto> GetTripDistances(int categoryId, DateTime fromTime, DateTime toTime) => 
        GetGroupedEventsByVehicle(categoryId, fromTime, toTime)
            .Select(group => new TripDistanceDto
            {
                VehicleId = group.Key,
                Distance = group.CalculateTripDistance()
            })
            .Where(dto => dto.Distance > 0)
            .OrderByDescending(dto => dto.Distance)
            .ToList();

    private IEnumerable<IGrouping<int, Event>> GetGroupedEventsByVehicle(int categoryId, DateTime fromTime, DateTime toTime)
    {
        return dataStore.Vehicles
            .Where(v => v.CategoryId == categoryId)
            .Join(dataStore.Events,
                  v => v.VehicleId,
                  e => e.VehicleId,
                  (v, e) => new { v.VehicleId, Event = e })
            .Where(x => x.Event.Timestamp >= fromTime && x.Event.Timestamp <= toTime)
            .OrderBy(x => x.VehicleId).ThenBy(x => x.Event.Timestamp)
            .GroupBy(x => x.VehicleId, x => x.Event);
    }
}
