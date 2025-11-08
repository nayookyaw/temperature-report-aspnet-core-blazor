using BackendAspNetCore.Dtos.SensorLog;

namespace BackendAspNetCore.Dtos.Sensor;
public class SensorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
    public DateTimeOffset LastUpdatedUtc { get; set; }

    public List<SensorLogDto> SensorLogs { get; set; } = new();
}