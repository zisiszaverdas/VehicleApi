using VehicleApi.Middleware;
using VehicleApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDataStore, DataStore>();
builder.Services.AddSingleton<IDataLoader, DataLoader>();
builder.Services.AddSingleton<ICategoryReportService, CategoryReportService>();
builder.Services.AddSingleton<IVehicleReportService, VehicleReportService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Load reference data on startup
using (var scope = app.Services.CreateScope())
{
    var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
    var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();
    var basePath = Path.Combine(AppContext.BaseDirectory, "Data");
    var categoryPath = Path.Combine(basePath, "CategoryData.dat");
    var vehiclePath = Path.Combine(basePath, "VehicleData.dat");
    if (File.Exists(categoryPath))
        dataStore.Categories = dataLoader.LoadCategories(categoryPath);
    if (File.Exists(vehiclePath))
        dataStore.Vehicles = dataLoader.LoadVehicles(vehiclePath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
