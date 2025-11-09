
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Mappers;
using BackendAspNetCore.Models;
using BackendAspNetCore.Repositories.SensorRepo;
using BackendAspNetCore.Repositories.SensorLogRepo;
using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Utils;
using BackendAspNetCore.RequestBody.Sensor;
using ZiggyCreatures.Caching.Fusion;

namespace BackendAspNetCore.Services.SensorServices;

public class SensorService(
    ISensorRepository iSensorRepo,
    ISensorLogRepository iSensorLogRepo,
    IFusionCache iFusionCache
) : ISensorService
{
    private readonly ISensorRepository _iSensorRepo = iSensorRepo;
    private readonly ISensorLogRepository _iSensorLogRepo = iSensorLogRepo;
    private readonly IFusionCache _iFusionCache = iFusionCache;

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
        await _iSensorLogRepo.SaveSensor(newSensorLog);
    }
    
    public async Task<ApiResponse<PagedResult<SensorDto>>> SearchSensorListAsync(ListSensorRequestBody input)
    {
        var page = input.Page;
        var pageSize = input.PageSize;
        var (sensorList, total) = await _iSensorRepo.SearchSensorListAsync(input.SearchText, page, pageSize);
        var payload = new PagedResult<SensorDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = sensorList.ToList()
        };
        return ApiResponse<PagedResult<SensorDto>>.SuccessResponse(
            payload,
            "Sensor list has been retrieved",
            200
        );
    }
    
    public async Task<ApiResponse<IEnumerable<SensorDto>>> GetSensorsInViewportAsync(ViewportSensorRequestBody input)
    {
        var minLng = input.MinLng;
        var minLat = input.MinLat;
        var maxLng = input.MaxLng;
        var maxLat = input.MaxLat;
        var zoom = input.Zoom;
        var limit = input.Limit;
        var search = input.Search ?? "";

        var cacheKey = MakeViewportKey(search, minLng, minLat, maxLng, maxLat, zoom, limit);

        var options = new FusionCacheEntryOptions()
            .SetDuration(TimeSpan.FromSeconds(60))   // short TTL for live map freshness
            .SetFailSafe(true);                     // use stale data if DB fails

        var cached = await _iFusionCache.TryGetAsync<IEnumerable<SensorDto>>(cacheKey);
        if (cached.HasValue)
        {
            Console.WriteLine("Cache hit  ✅ ✅ ✅ ✅");
            return ApiResponse<IEnumerable<SensorDto>>.SuccessResponse(
                cached.Value,
                "Retrieved from cache",
                200
            );
        }

        Console.WriteLine("Cache miss ❌, querying DB...");
        var sensorList = await _iSensorRepo.GetSensorsInViewportAsync(minLng, minLat, maxLng, maxLat, zoom, limit, search);
        await _iFusionCache.SetAsync(cacheKey, sensorList, options);
        return ApiResponse<IEnumerable<SensorDto>>.SuccessResponse(
            sensorList,
            "Sensor list in viewport has been retrieved",
            200
        );
    }
    
    private static string MakeViewportKey(string search, double minLng, double minLat, double maxLng, double maxLat, int zoom, int limit)
    {
        static double Q(double v) => Math.Round(v, 3); // keep some precision, but not too coarse
        var s = (search ?? "").Trim().ToLowerInvariant();

        return $"vp:min({Q(minLng)},{Q(minLat)})"
            + $":max({Q(maxLng)},{Q(maxLat)})"
            + $":z{zoom}"
            + $":l{limit}"
            + $":s:{s}";
    }
}