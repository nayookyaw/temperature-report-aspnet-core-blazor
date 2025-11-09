
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
        string searchText = queryParams.SearchText ?? string.Empty;
        int page = queryParams.Page;
        int pageSize = queryParams.PageSize;
        
        page = Math.Max(page, 1);
        pageSize = pageSize is < 1 or > 200 ? 50 : pageSize;

        var allSensor = await _iSensorService.SearchAsync(searchText, int.MaxValue);
        var total = allSensor.Count();

        var items = allSensor.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var result = new PagedResult<SensorDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };

        return Ok(result);
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