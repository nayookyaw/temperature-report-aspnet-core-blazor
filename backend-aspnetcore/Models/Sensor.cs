using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAspNetCore.Models;

public class Sensor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MacAddress { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public double  Temperature { get; set; }

    public double  Humidity { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Status { get; set; }

    public DateTimeOffset? LastSeenAt { get; set; }

    public DateTimeOffset LastUpdatedUtc { get; set; }

    public virtual ICollection<SensorLog> SensorLogs { get; set; } = new List<SensorLog>();
}