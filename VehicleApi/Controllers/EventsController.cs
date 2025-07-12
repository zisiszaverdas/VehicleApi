using Microsoft.AspNetCore.Mvc;
using VehicleApi.Services;

namespace VehicleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IDataStore dataStore, IDataLoader dataLoader, ILogger<EventsController> logger) : ControllerBase
{

    [RequestSizeLimit(200_000_000)] // increased limit to 200 MB
    [HttpPost("upload")]
    public async Task<IActionResult> UploadEvents([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var tempFilePath = Path.GetTempFileName();
        try
        {
            using (var stream = System.IO.File.Create(tempFilePath))
            {
                await file.CopyToAsync(stream);
            }
            var events = dataLoader.LoadEvents(tempFilePath);

            foreach (var eventItem in events)
            {
                dataStore.Events.Add(eventItem);
            }

            return Ok(new { events.Count });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upload events file.");
            return StatusCode(500, "Failed to process the file.");
        }
        finally
        {
            if (System.IO.File.Exists(tempFilePath))
                System.IO.File.Delete(tempFilePath);
        }
    }
}
