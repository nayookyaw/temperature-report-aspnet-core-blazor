using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.SensorLogRepo;

public interface ISensorLogRepository
{
    public Task<SensorLog> SaveSensor(SensorLog newSensorLog);
}