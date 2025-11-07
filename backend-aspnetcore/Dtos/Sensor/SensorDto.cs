namespace BackendAspNetCore.Dtos.Sensor;
public class SensorDto
{
    public string MacAddress { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public DateTimeOffset LastUpdatedUtc { get; set; }
}