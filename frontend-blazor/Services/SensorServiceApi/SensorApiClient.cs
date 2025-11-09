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
        var envelope = await _http.GetFromJsonAsync<ApiResponse<PagedResult<SensorDto>>>(sensorListEndpoint, ct);
        var payload = envelope?.Data 
            ?? new PagedResult<SensorDto>(Array.Empty<SensorDto>(), page, pageSize, 0);

        return payload;
    }
}
