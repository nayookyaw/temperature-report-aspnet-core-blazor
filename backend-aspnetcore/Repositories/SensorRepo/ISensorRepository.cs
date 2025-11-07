
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.SensorRepo;

public interface ISensorRepository
{
    public Task<Sensor> SaveSensor(Sensor newSensor);
    public Task<Sensor> UpdateSensor(Sensor sensor);
    public Task<Sensor?> GetSensorByMacAddress(string macAddress);
    public Task<IEnumerable<SensorDto>> GetAllSensor();
}