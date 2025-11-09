
using System.Net.Http.Json;

namespace Frontend.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly string _base;

    public ApiClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _base = cfg["BackendBase"] ?? "";
    }

    public async Task<T?> GetJsonAsync<T>(string pathAndQuery)
        => await _http.GetFromJsonAsync<T>($"{_base.TrimEnd('/')}/{pathAndQuery.TrimStart('/')}");
}
