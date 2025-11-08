
namespace BackendAspNetCore.Dtos.Sensor;

public class ViewportQuery
{
    public double MinLng { get; set; }
    public double MinLat { get; set; }
    public double MaxLng { get; set; }
    public double MaxLat { get; set; }
    public int Zoom { get; set; } = 5;
    public int Limit { get; set; } = 5000;
    public string? Search { get; set; }
}
