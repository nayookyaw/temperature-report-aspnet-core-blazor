using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAspNetCore.Models;

public class Sensor
{
    public Guid Id { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public DateTimeOffset LastUpdatedUtc { get; set; }
}