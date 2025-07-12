using VehicleApi.DTOs;

namespace VehicleApi.Services;

public interface ICategoryReportService
{
    IEnumerable<ViolationDto> GetViolations(int categoryId, DateTime fromTime, DateTime toTime);
    IEnumerable<TripDistanceDto> GetTripDistances(int categoryId, DateTime fromTime, DateTime toTime);
}
