using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using VehicleApi.Controllers;
using VehicleApi.Services;
using VehicleApi.Models;

namespace VehicleApi.Tests.Controllers;

public class EventsControllerTests
{
    private readonly IDataStore _dataStore;
    private readonly IDataLoader _dataLoader;
    private readonly ILogger<EventsController> _logger;
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        _dataStore = Substitute.For<IDataStore>();
        _dataStore.Events.Returns(new List<Event>());
        _dataLoader = Substitute.For<IDataLoader>();
        _logger = Substitute.For<ILogger<EventsController>>();
        _controller = new EventsController(_dataStore, _dataLoader, _logger);
    }

    [Fact]
    public async Task UploadEvents_ReturnsBadRequest_WhenFileIsNull()
    {
        var result = await _controller.UploadEvents(null);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file uploaded.", badRequest.Value);
    }

    [Fact]
    public async Task UploadEvents_ReturnsBadRequest_WhenFileIsEmpty()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(0);
        var result = await _controller.UploadEvents(file);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file uploaded.", badRequest.Value);
    }

    [Fact]
    public async Task UploadEvents_ReturnsOk_WhenFileIsValid()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(10);
        var stream = new MemoryStream(new byte[10]);
        file.CopyToAsync(Arg.Any<Stream>(), default).Returns(x => {
            var dest = (Stream)x[0];
            stream.Position = 0;
            stream.CopyTo(dest);
            return Task.CompletedTask;
        });
        var events = new List<Event> { new Event() };
        _dataLoader.LoadEvents(Arg.Any<string>()).Returns(events);

        var eventsList = new List<Event>();
        _dataStore.Events = eventsList;

        var result = await _controller.UploadEvents(file);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, (int)okResult.Value.GetType().GetProperty("Count")!.GetValue(okResult.Value)!);
        Assert.Single(eventsList);
    }

    [Fact]
    public async Task UploadEvents_ReturnsServerError_WhenExceptionThrown()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(10);
        file.CopyToAsync(Arg.Any<Stream>(), default).Returns(Task.CompletedTask);
        _dataLoader.LoadEvents(Arg.Any<string>()).Returns(x => throw new IOException("fail"));

        var result = await _controller.UploadEvents(file);
        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        Assert.Equal("Failed to process the file.", serverError.Value);
    }
}
