
using BackendAspNetCore.Data;
using BackendAspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAspNetCore.Repositories.SensorRepo;

public class SensorRepository(AppDbContext db) : ISensorRepository
{
    private readonly AppDbContext _db = db;

    public async Task<Sensor> SaveSensor(Sensor newSensor)
    {
        _db.Sensors.Add(newSensor);
        await _db.SaveChangesAsync();
        return newSensor;
    }

    public async Task<Sensor> UpdateSensor(Sensor updateSensor)
    {
        _db.Sensors.Update(updateSensor);
        await _db.SaveChangesAsync();
        return updateSensor;
    }

    public async Task<Sensor?> GetSensorByMacAddress(string macAddress)
    {
        return await _db.Sensors
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.MacAddress == macAddress);
    }

    public async Task<IEnumerable<Sensor>> GetAllSensor()
    {
        return await _db.Sensors.AsNoTracking().OrderBy(s => s.MacAddress).ToListAsync();
        // return await _db.Sensors
        //     .AsNoTracking()
        //     .OrderBy(s => s.MacAddress)
        //     .Select(s => new SensorDto
        //     {
        //         MacAddress   = s.MacAddress,
        //         Temperature  = s.Temperature.ToString(),   // adapt to your types/format
        //         Humidity     = s.Humidity.ToString(),
        //         // Example: include a DateTimeOffset in your DTO if you have it
        //         // LastSeen = s.LastSeen, 
        //     })
        //     .ToListAsync(ct);
    }
}