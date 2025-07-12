using Microsoft.AspNetCore.Mvc;
using VehicleApi.Services;
using VehicleApi.DTOs;

namespace VehicleApi.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController(IVehicleReportService vehicleReportService) : ControllerBase
{

    [HttpGet("{vehicleId}/route")]
    public ActionResult<RouteByVehicleDto> GetRouteByVehicle([FromRoute] int vehicleId, [FromQuery] DateTime fromTime, [FromQuery] DateTime toTime) =>
        Ok(vehicleReportService.GetRouteByVehicle(vehicleId, fromTime, toTime));
}
