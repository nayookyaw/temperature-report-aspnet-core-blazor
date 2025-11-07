
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Mappers;
using BackendAspNetCore.Models;
using BackendAspNetCore.Repositories.SensorRepo;
using BackendAspNetCore.Repositories.SensorLogRepo;
using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Utils;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Services.SensorServices;

public class SensorService(
    ISensorRepository iSensorRepo,
    ISensorLogRepository iSensorLogRepo
) : ISensorService
{
    private readonly ISensorRepository _iSensorRepo = iSensorRepo;
    private readonly ISensorLogRepository _iSensorLogRepo = iSensorLogRepo;

    public async Task<ApiResponse> SaveOrUpdateSensor(AddSensorRequestBody input)
    {
        SensorDto sensorDto;
        Sensor? existSensor = await _iSensorRepo.GetSensorByMacAddress(input.MacAddress);
        if (existSensor != null)
        {
            existSensor.Temperature = input.Temperature;
            existSensor.Humidity = input.Humidity;
            existSensor.LastUpdatedUtc = DatetimeUtil.GetCurrentUtcDatetime();
            await _iSensorRepo.UpdateSensor(existSensor);
            sensorDto = SensorMapper.ToDto(existSensor);
            SaveSensorLog(existSensor);
            return ApiResponse<SensorDto>.SuccessResponse(sensorDto, "Sensor has been updated", 200);
        }
        var newSensor = new Sensor
        {
            MacAddress = input.MacAddress,
            Temperature = input.Temperature,
            Humidity = input.Humidity,
            LastUpdatedUtc = DatetimeUtil.GetCurrentUtcDatetime(),
        };
        Sensor sensor = await _iSensorRepo.SaveSensor(newSensor);
        sensorDto = SensorMapper.ToDto(sensor);
        SaveSensorLog(sensor);
        return ApiResponse<SensorDto>.SuccessResponse(sensorDto, "New sensor has been added", 200);
    }
    private async void SaveSensorLog(Sensor sensor)
    {
        var newSensorLog = new SensorLog
        {
            SensorId = sensor.Id,
            Temperature = sensor.Temperature,
            Humidity = sensor.Humidity,
        };
        SensorLog sensorLog = await _iSensorLogRepo.SaveSensor(newSensorLog);
    }
    
    public async Task<ApiResponse> GetAllSensor(GetAllSensorRequestBody input)
    {
        IEnumerable<SensorDto> sensorList = await _iSensorRepo.GetAllSensor();
        if (!sensorList.Any())
        {
            return ApiResponseFail.FailResponse("No sensor is found", 400);
        }
        return ApiResponse<IEnumerable<SensorDto>>.SuccessResponse(sensorList, "Sensor list has been retrieved", 200);
    }
}