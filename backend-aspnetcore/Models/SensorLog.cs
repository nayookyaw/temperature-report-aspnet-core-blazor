namespace BackendAspNetCore.Models;

public class SensorLog
{
    public Guid Id { get; set; }
    public Guid SensorId { get; set; }
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    // lazy-loaded for sensor
    public virtual Sensor Sensor { get; set; } = null!;
}