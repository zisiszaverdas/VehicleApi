using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VehicleApi.DTOs;
using VehicleApi.Services;

namespace VehicleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryReportService categoryReportService) : ControllerBase
{
    [HttpGet("{categoryId}/violations")]
    public ActionResult<IEnumerable<ViolationDto>> GetViolations(
        [FromRoute] int categoryId,
        [FromQuery, Required] DateTime fromTime,
        [FromQuery, Required] DateTime toTime) =>
        Ok(categoryReportService.GetViolations(categoryId, fromTime, toTime));

    [HttpGet("{categoryId}/trip-distances")]
    public ActionResult<IEnumerable<TripDistanceDto>> GetTripDistances(
        [FromRoute] int categoryId,
        [FromQuery, Required] DateTime fromTime, 
        [FromQuery, Required] DateTime toTime) =>
        Ok(categoryReportService.GetTripDistances(categoryId, fromTime, toTime));
}
