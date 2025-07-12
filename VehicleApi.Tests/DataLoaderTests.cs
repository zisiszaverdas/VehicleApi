using Microsoft.Extensions.Logging;
using NSubstitute;
using VehicleApi.Services;

namespace VehicleApi.Tests;

public class DataLoaderTests : IDisposable
{
    private readonly ILogger<DataLoader> _logger;
    private readonly DataLoader _dataLoader;
    private readonly string _testDir;

    public DataLoaderTests()
    {
        _logger = Substitute.For<ILogger<DataLoader>>();
        _dataLoader = new DataLoader(_logger);
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void LoadCategories_ValidFile_ReturnsCategories()
    {
        var file = Path.Combine(_testDir, "categories.dat");
        File.WriteAllLines(file, new[]
        {
            "CategoryId\tSpeedLimitKm\tSpeedLimitDurationSeconds",
            "1\t50\t60",
            "2\t80\t120"
        });
        var result = _dataLoader.LoadCategories(file);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].CategoryId);
        Assert.Equal(80, result[1].SpeedLimitKm);
    }

    [Fact]
    public void LoadVehicles_ValidFile_ReturnsVehicles()
    {
        var file = Path.Combine(_testDir, "vehicles.dat");
        File.WriteAllLines(file, new[]
        {
            "VehicleId\tCategoryId",
            "1\t2",
            "2\t3"
        });
        var result = _dataLoader.LoadVehicles(file);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].CategoryId);
    }

    [Fact]
    public void LoadEvents_ValidFile_ReturnsEvents()
    {
        var file = Path.Combine(_testDir, "events.dat");
        File.WriteAllLines(file, new[]
        {
            "VehicleId\tTimestamp\tSpeedKm\tLatitude\tLongitude",
            $"1\t{DateTime.UtcNow:O}\t100.5\t10.1\t20.2"
        });
        var result = _dataLoader.LoadEvents(file);
        Assert.Single(result);
        Assert.Equal(1, result[0].VehicleId);
    }

    [Fact]
    public void LoadCategories_FileNotFound_LogsWarningAndReturnsEmpty()
    {
        var file = Path.Combine(_testDir, "notfound.dat");
        var result = _dataLoader.LoadCategories(file);
        Assert.Empty(result);
        _logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("File not found")),
            null,
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public void LoadCategories_MalformedLine_SkipsLine()
    {
        var file = Path.Combine(_testDir, "malformed.dat");
        File.WriteAllLines(file, new[]
        {
            "CategoryId\tSpeedLimitKm\tSpeedLimitDurationSeconds",
            "bad\tdata\trow",
            "2\t80\t120"
        });
        var result = _dataLoader.LoadCategories(file);
        Assert.Single(result);
        Assert.Equal(2, result[0].CategoryId);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}
