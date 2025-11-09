namespace Frontend.Dtos;
public record SensorDto(
    string Id,
    string Name,
    string MacAddress,
    string SerialNumber,
    string Status,
    double Latitude,
    double Longitude,
    double Temperature,
    double Humidity,
    string UpdatedAt
);