namespace Frontend.Models;

public record SensorDto(
    string id,
    string macAddress,
    string serialNumber,
    string name,
    double latitude,
    double longitude,
    double temperature,
    double humidity,
    string updatedAt
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalCount
);
