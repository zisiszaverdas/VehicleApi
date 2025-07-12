using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using VehicleApi.Controllers;
using VehicleApi.DTOs;
using VehicleApi.Services;

namespace VehicleApi.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly ICategoryReportService _service;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _service = Substitute.For<ICategoryReportService>();
        _controller = new CategoriesController(_service);
    }

    [Fact]
    public void GetViolations_ReturnsOk_WithExpectedDtos()
    {
        int categoryId = 1;
        var from = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var to = from.AddHours(1);
        var expected = new List<ViolationDto> { new ViolationDto { VehicleId = 42, CountViolations = 2 } };
        _service.GetViolations(categoryId, from, to).Returns(expected);

        var result = _controller.GetViolations(categoryId, from, to);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ViolationDto>>(ok.Value);
        Assert.Single(dtos);
        Assert.Equal(42, dtos.First().VehicleId);
        _service.Received(1).GetViolations(categoryId, from, to);
    }

    [Fact]
    public void GetTripDistances_ReturnsOk_WithExpectedDtos()
    {
        int categoryId = 1;
        var from = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var to = from.AddHours(1);
        var expected = new List<TripDistanceDto> { new TripDistanceDto { VehicleId = 42, Distance = 123.4 } };
        _service.GetTripDistances(categoryId, from, to).Returns(expected);

        var result = _controller.GetTripDistances(categoryId, from, to);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<TripDistanceDto>>(ok.Value);
        Assert.Single(dtos);
        Assert.Equal(42, ((TripDistanceDto)dtos.First()).VehicleId);
        _service.Received(1).GetTripDistances(categoryId, from, to);
    }
}
