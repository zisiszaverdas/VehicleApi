namespace VehicleApi.Services;

public class GpsEventGeneratorBackgroundService(IDataStore dataStore, IGpsEventGeneratorService gpsEventGeneratorService) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            gpsEventGeneratorService.GenerateEvents(dataStore);
            await Task.Delay(TimeSpan.FromSeconds(GpsEventGeneratorService.EventIntervalSeconds), stoppingToken);
        }
    }
}

