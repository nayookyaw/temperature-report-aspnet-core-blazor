using Frontend.Dtos;

namespace Frontend.Services.SensorServiceApi;

public interface ISensorApiClient
{
    Task<PagedResult<SensorDto>> SearchSensorList(string? searchText, int page, int pageSize, CancellationToken ct = default);
}
