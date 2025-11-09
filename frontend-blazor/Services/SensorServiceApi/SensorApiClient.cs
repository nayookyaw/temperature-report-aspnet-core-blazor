using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services.SensorServiceApi;

public sealed class SensorApiClient : ISensorApiClient
{
    private readonly HttpClient _http;

    public SensorApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResult<SensorDto>> SearchSensorList(string? q, int page, int pageSize, CancellationToken ct = default)
    {
        var qEncoded = string.IsNullOrWhiteSpace(q) ? "" : Uri.EscapeDataString(q);
        var url = $"api/v1/sensor/list?q={qEncoded}&page={page}&pageSize={pageSize}";

        // If backend returns null or malformed JSON, protect the UI
        var result = await _http.GetFromJsonAsync<PagedResult<SensorDto>>(url, ct);
        return result ?? new PagedResult<SensorDto>(Array.Empty<SensorDto>(), page, pageSize, 0);
    }
}
