using Frontend.Models;

namespace Frontend.Services.SensorServiceApi;

public interface ISensorApiClient
{
    Task<PagedResult<SensorDto>> SearchSensorList(string? q, int page, int pageSize, CancellationToken ct = default);
}
