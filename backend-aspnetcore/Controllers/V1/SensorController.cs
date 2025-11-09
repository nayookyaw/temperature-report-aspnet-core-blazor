
using Microsoft.AspNetCore.Mvc;
using BackendAspNetCore.Services.SensorServices;
using BackendAspNetCore.RequestBody.Sensor;

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
        var response = await _iSensorService.SaveOrUpdateSensor(input);
        return StatusCode(response.StatusCode, response);
    }

    // GET v1/sensor/list?searchText=&page=1&pageSize=50
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] ListSensorRequestBody queryParams)
    {
        var response = await _iSensorService.SearchSensorListAsync(queryParams);
        return StatusCode(response.StatusCode, response);
    }

    // GET v1/sensor/viewport?minLng=&minLat=&maxLng=&maxLat=&zoom=&limit=&search=
    [HttpGet("viewport")]
    public async Task<IActionResult> Viewport([FromQuery] ViewportSensorRequestBody queryParams)
    {
        var response = await _iSensorService.GetSensorsInViewportAsync(queryParams);
        return StatusCode(response.StatusCode, response);
    }
}