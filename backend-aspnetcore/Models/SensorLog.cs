namespace BackendAspNetCore.Models;

public class SensorLog
{
    public Guid Id { get; set; }
    public Guid SensorId { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // lazy-loaded for sensor
    public virtual Sensor Sensor { get; set; } = null!;
}