namespace BackendAspNetCore.RequestBody.Sensor;

public class ViewportSensorRequestBody
{
    public double MinLng { get; set; }
    public double MinLat { get; set; }
    public double MaxLng { get; set; }
    public double MaxLat { get; set; }
    public int Zoom { get; set; }
    public int Limit { get; set; }
    public string? Search { get; set; } = string.Empty;
}