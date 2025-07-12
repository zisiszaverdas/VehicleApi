using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VehicleApi.Services;
using VehicleApi.DTOs;

namespace VehicleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(IVehicleReportService vehicleReportService) : ControllerBase
{

    [HttpGet("{vehicleId}/route")]
    public ActionResult<RouteByVehicleDto> GetRouteByVehicle(
        [FromRoute] int vehicleId,
        [FromQuery, Required] DateTime fromTime,
        [FromQuery, Required] DateTime toTime) =>
        Ok(vehicleReportService.GetRouteByVehicle(vehicleId, fromTime, toTime));
}
