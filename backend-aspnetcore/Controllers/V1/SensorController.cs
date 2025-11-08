
using Microsoft.AspNetCore.Mvc;
using BackendAspNetCore.Services.SensorServices;
using BackendAspNetCore.RequestBody.Sensor;
using BackendAspNetCore.Dtos.Sensor;

namespace BackendAspNetCore.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/sensor")]
public class SensorController(ISensorService iSensorService) : ControllerBase
{
    private readonly ISensorService _iSensorService = iSensorService;

    [HttpPost]
    public async Task<IActionResult> SaveOrUpdateSensor([FromBody] AddSensorRequestBody input)
    {
        var response = await _iSensorService.SaveOrUpdateSensor(input);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("list")]
    public async Task<ActionResult> GetAllSensor([FromQuery] GetAllSensorRequestBody input)
    {
        var response = await _iSensorService.GetAllSensor(input);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("viewport")]
    public async Task<IActionResult> GetByViewport(
        [FromQuery] double minLng,
        [FromQuery] double minLat,
        [FromQuery] double maxLng,
        [FromQuery] double maxLat,
        [FromQuery] int zoom = 5,
        [FromQuery] int limit = 5000,
        [FromQuery] string? search = null)
    {
        var result = await _iSensorService.GetSensorsInViewportAsync(new ViewportQuery
        {
            MinLng = minLng, MinLat = minLat, MaxLng = maxLng, MaxLat = maxLat,
            Zoom = zoom, Limit = Math.Clamp(limit, 100, 20000), Search = search
        });

        return Ok(result);
    }

    // Quick search for the left panel list (prefix/contains search by name/mac)
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 50)
        => Ok(await _iSensorService.SearchAsync(q ?? string.Empty, Math.Clamp(limit, 5, 200)));
}