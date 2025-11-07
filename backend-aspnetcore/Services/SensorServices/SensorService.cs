
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Mappers;
using BackendAspNetCore.Models;
using BackendAspNetCore.Repositories.SensorRepo;
using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Utils;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Services.SensorServices;

public class SensorService(ISensorRepository iSensorRepo) : ISensorService
{
    private readonly ISensorRepository _iSensorRepo = iSensorRepo;

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
            Console.WriteLine($"update {input.Temperature}");

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
        return ApiResponse<SensorDto>.SuccessResponse(sensorDto, "New sensor has been added", 200);
    }
    
    public async Task<ApiResponse> GetAllSensor(GetAllSensorRequestBody input)
    {
        IEnumerable<Sensor> sensors = await _iSensorRepo.GetAllSensor();
        List<SensorDto> sensorList = sensors.Select(s => SensorMapper.ToDto(s)).ToList();
        if (sensorList.Count == 0)
        {
            return ApiResponseFail.FailResponse("No sensor is found", 400);
        }
        return ApiResponse<List<SensorDto>>.SuccessResponse(sensorList, "Sensor list has been retrieved", 200);
    }
}