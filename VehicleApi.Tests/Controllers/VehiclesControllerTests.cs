using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using VehicleApi.Controllers;
using VehicleApi.DTOs;
using VehicleApi.Services;

namespace VehicleApi.Tests.Controllers;

public class VehiclesControllerTests
{
    private readonly IVehicleReportService _service;
    private readonly VehiclesController _controller;

    public VehiclesControllerTests()
    {
        _service = Substitute.For<IVehicleReportService>();
        _controller = new VehiclesController(_service);
    }

    [Fact]
    public void GetRouteByVehicle_ReturnsOk_WithExpectedDto()
    {
        // Arrange
        int vehicleId = 42;
        var from = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var to = from.AddHours(1);
        var expected = new RouteByVehicleDto { VehicleId = vehicleId, TripDistance = 123.4 };
        _service.GetRouteByVehicle(vehicleId, from, to).Returns(expected);

        // Act
        var result = _controller.GetRouteByVehicle(vehicleId, from, to);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<RouteByVehicleDto>(ok.Value);
        Assert.Equal(vehicleId, dto.VehicleId);
        Assert.Equal(123.4, dto.TripDistance);
        _service.Received(1).GetRouteByVehicle(vehicleId, from, to);
    }
}
