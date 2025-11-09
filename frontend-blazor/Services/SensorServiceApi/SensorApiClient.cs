using System.Net.Http.Json;
using Frontend.Dtos;

namespace Frontend.Services.SensorServiceApi;

public sealed class SensorApiClient : ISensorApiClient
{
    private readonly HttpClient _http;

    public SensorApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResult<SensorDto>> SearchSensorList(string? searchText, int page, int pageSize, CancellationToken ct = default)
    {
        var searchTextEncoded = string.IsNullOrWhiteSpace(searchText) ? "" : Uri.EscapeDataString(searchText);
        var sensorListEndpoint = $"v1/sensor/list?searchText={searchTextEncoded}&page={page}&pageSize={pageSize}";

        // If backend returns null or malformed JSON, protect the UI
        var result = await _http.GetFromJsonAsync<PagedResult<SensorDto>>(sensorListEndpoint, ct);
        return result ?? new PagedResult<SensorDto>(Array.Empty<SensorDto>(), page, pageSize, 0);
    }
}
