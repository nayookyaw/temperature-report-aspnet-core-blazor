namespace BackendAspNetCore.RequestBody.Sensor;

public class ListSensorRequestBody
{
    public string? SearchText { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}