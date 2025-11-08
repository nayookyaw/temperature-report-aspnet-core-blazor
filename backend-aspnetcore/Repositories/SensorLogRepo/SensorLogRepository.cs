using BackendAspNetCore.Data;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.SensorLogRepo;

public class SensorLogRepository(AppDbContext db) : ISensorLogRepository
{
    private readonly AppDbContext _db = db;
    public async Task<SensorLog> SaveSensor(SensorLog newSensorLog)
    {
        _db.SensorLogs.Add(newSensorLog);
        await _db.SaveChangesAsync();
        return newSensorLog;
    }
}