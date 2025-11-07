
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
}