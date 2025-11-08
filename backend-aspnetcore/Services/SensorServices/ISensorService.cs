
using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Services.SensorServices;
public interface ISensorService
{
    public Task<ApiResponse> SaveOrUpdateSensor(AddSensorRequestBody input);
    public Task<ApiResponse> GetAllSensor(GetAllSensorRequestBody input);
    Task<IEnumerable<SensorDto>> GetSensorsInViewportAsync(ViewportQuery q);
    Task<IEnumerable<SensorDto>> SearchAsync(string q, int limit);
}