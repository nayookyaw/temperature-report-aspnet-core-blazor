
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Services.SensorServices;
public interface ISensorService
{
    public Task<ApiResponse> SaveOrUpdateSensor(AddSensorRequestBody input);
    public Task<ApiResponse> GetAllSensor(GetAllSensorRequestBody input);
}