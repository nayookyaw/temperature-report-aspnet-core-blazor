namespace Frontend.Models;

public record SensorDto(
    string Id,
    string Name,
    string MacAddress,
    string SerialNumber,
    double Latitude,
    double Longitude,
    double Temperature,
    double Humidity,
    string UpdatedAt
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalCount
);
