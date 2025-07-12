namespace VehicleApi.Services
{
    public interface IGpsEventGeneratorService
    {
        void GenerateEvents(IDataStore dataStore);
    }
}