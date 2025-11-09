
using Microsoft.AspNetCore.Mvc;
using BackendAspNetCore.Services.SensorServices;
using BackendAspNetCore.RequestBody.Sensor;
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Dtos.Response;
using System.Text.Json.Nodes;

namespace BackendAspNetCore.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/sensor")]
public class SensorController(ISensorService iSensorService) : ControllerBase
{
    private readonly ISensorService _iSensorService = iSensorService;

    // POST v1/sensor
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] AddSensorRequestBody input)
    {
        return StatusCode((await _iSensorService.SaveOrUpdateSensor(input)).StatusCode);
    }

    // GET v1/sensor/list?searchText=&page=1&pageSize=50
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] ListSensorRequestBody queryParams)
    {
        var resp = await _iSensorService.SearchSensorListAsync(queryParams);
        return StatusCode(resp.StatusCode, resp);
    }

    // GET v1/sensor/viewport?minLng=&minLat=&maxLng=&maxLat=&zoom=&limit=&search=
    [HttpGet("viewport")]
    public async Task<IActionResult> Viewport([FromQuery] double minLng, [FromQuery] double minLat,
                                              [FromQuery] double maxLng, [FromQuery] double maxLat,
                                              [FromQuery] int zoom, [FromQuery] int limit = 1000,
                                              [FromQuery] string? search = null)
    {
        var result = await _iSensorService.GetSensorsInViewportAsync(new ViewportQuery
        {
            MinLng = minLng, MinLat = minLat, MaxLng = maxLng, MaxLat = maxLat,
            Zoom = zoom, Limit = Math.Clamp(limit, 100, 20000), Search = search
        });
        return Ok(result);
    }
}