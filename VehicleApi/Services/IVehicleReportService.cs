using VehicleApi.DTOs;

namespace VehicleApi.Services;

public interface IVehicleReportService
{
    RouteByVehicleDto GetRouteByVehicle(int vehicleId, DateTime fromTime, DateTime toTime);
}
