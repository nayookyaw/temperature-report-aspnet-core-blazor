
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendAspNetCore.Data;
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAspNetCore.Repositories.SensorRepo;

public class SensorRepository(AppDbContext db, IMapper mapper) : ISensorRepository
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

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

    public async Task<SensorDto?> GetSensorByMacAddress(string macAddress)
    {
        return await _db.Sensors
            .AsNoTracking()
            .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(s => s.MacAddress == macAddress);
    }

    public async Task<IEnumerable<SensorDto>> GetAllSensor()
    {
        return await _db.Sensors
            .AsNoTracking()
            .OrderBy(s => s.MacAddress)
            .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}