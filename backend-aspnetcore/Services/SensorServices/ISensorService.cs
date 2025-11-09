
using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Services.SensorServices;
public interface ISensorService
{
    public Task<ApiResponse> SaveOrUpdateSensor(AddSensorRequestBody input);
    public Task<ApiResponse<PagedResult<SensorDto>>> SearchSensorListAsync(ListSensorRequestBody input);
    public Task<ApiResponse<IEnumerable<SensorDto>>> GetSensorsInViewportAsync(ViewportSensorRequestBody input);
}