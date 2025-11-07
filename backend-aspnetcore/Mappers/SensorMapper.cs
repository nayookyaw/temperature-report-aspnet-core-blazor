using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Mappers;

public static class SensorMapper
{
    public static SensorDto ToDto(Sensor sensor) => new SensorDto
    {
        MacAddress = sensor.MacAddress,
        Temperature = sensor.Temperature,
        Humidity = sensor.Humidity,
        LastUpdatedUtc = sensor.LastUpdatedUtc,
    };
}