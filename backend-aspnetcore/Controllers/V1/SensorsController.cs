
using Microsoft.AspNetCore.Mvc;
using BackendAspNetCore.Services.SensorServices;
using BackendAspNetCore.Dtos.Sensor;

namespace BackendAspNetCore.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/sensors")]
public class SensorsController(ISensorService svc) : ControllerBase
{
    [HttpGet("viewport")]
    public async Task<IActionResult> Viewport([FromQuery] double minLng, [FromQuery] double minLat,
                                              [FromQuery] double maxLng, [FromQuery] double maxLat,
                                              [FromQuery] int zoom, [FromQuery] int limit = 1000,
                                              [FromQuery] string? search = null)
    {
        var result = await svc.GetSensorsInViewportAsync(new ViewportQuery
        {
            MinLng = minLng, MinLat = minLat, MaxLng = maxLng, MaxLat = maxLat,
            Zoom = zoom, Limit = Math.Clamp(limit, 100, 20000), Search = search
        });
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int limit = 50)
        => Ok(await svc.SearchAsync(q ?? string.Empty, Math.Clamp(limit, 5, 200)));
}
