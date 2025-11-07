namespace BackendAspNetCore.Dtos.SensorLog;

public class SensorLogDto
{
    public Guid Id { get; set; }
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}