
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.SensorRepo;

public interface ISensorRepository
{
    public Task<Sensor?> GetSensorByMacAddress(string macAddress);
    public Task<Sensor> SaveSensor(Sensor newSensor);
    public Task<Sensor> UpdateSensor(Sensor sensor);
    public Task<(IEnumerable<SensorDto> Items, long TotalCount)> SearchSensorListAsync
    (
        string? searchText, int page, int pageSize, CancellationToken ct = default
    );
    public Task<IEnumerable<SensorDto>> GetSensorsInViewportAsync
    (
        double minLng, double minLat,
        double maxLng, double maxLat,
        int zoom, int limit, string? search,
        CancellationToken ct = default
    );
}