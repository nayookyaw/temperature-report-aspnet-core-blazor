
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.SensorRepo;

public interface ISensorRepository
{
    public Task<Sensor> SaveSensor(Sensor newSensor);
    public Task<Sensor> UpdateSensor(Sensor sensor);
    public Task<Sensor?> GetSensorByMacAddress(string macAddress);
    Task<IEnumerable<SensorDto>> GetSensorsInViewportAsync(ViewportQuery q);
    public Task<(IEnumerable<SensorDto> Items, long TotalCount)> SearchSensorListAsync(
        string? searchText, int page, int pageSize, CancellationToken ct = default
    );
}