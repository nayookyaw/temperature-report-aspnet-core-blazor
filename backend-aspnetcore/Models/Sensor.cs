using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAspNetCore.Models;

public class Sensor
{
    [Column(Order = 0)]
    public Guid Id { get; set; }

    [Column(Order = 1)]
    public string MacAddress { get; set; } = string.Empty;

    [Column(Order = 2)]
    public string SerialNumber { get; set; } = string.Empty;

    [Column(Order = 3)]
    public string Temperature { get; set; } = string.Empty;

    [Column(Order = 4)]
    public string Humidity { get; set; } = string.Empty;

    [Column(Order = 5)]
    public DateTimeOffset LastUpdatedUtc { get; set; }
}