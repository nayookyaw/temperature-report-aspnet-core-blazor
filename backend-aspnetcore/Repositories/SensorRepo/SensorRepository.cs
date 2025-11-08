
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

    public async Task<Sensor?> GetSensorByMacAddress(string macAddress)
    {
        return await _db.Sensors
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.MacAddress == macAddress);
    }

    public async Task<IEnumerable<SensorDto>> GetAllSensor(CancellationToken ct = default)
    {
        return await _db.Sensors
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(s => s.MacAddress)
            .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
    
    public async Task<IEnumerable<SensorDto>> GetSensorsInViewportAsync(ViewportQuery q)
    {
        var query = _db.Sensors
            .AsNoTracking()
            .Where(s => s.Longitude >= q.MinLng && s.Longitude <= q.MaxLng
                        && s.Latitude >= q.MinLat && s.Latitude <= q.MaxLat);

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(s) || x.MacAddress.ToLower().Contains(s));
        }

        query = query.OrderByDescending(s => s.LastSeenAt); // prioritize fresh
        if (q.Limit > 0) query = query.Take(q.Limit);

        return await query
            .Select(s => new SensorDto
            {
                Id = s.Id,
                Name = s.Name,
                MacAddress = s.MacAddress,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Status = s.Status,
                LastSeenAt = s.LastSeenAt
            }).ToListAsync();
    }

    public async Task<IEnumerable<SensorDto>> SearchAsync(string q, int limit)
    {
        q = (q ?? string.Empty).Trim().ToLower();
        var query = _db.Sensors.AsNoTracking();

        if (!string.IsNullOrEmpty(q))
            query = query.Where(s => s.Name.ToLower().Contains(q) || s.MacAddress.ToLower().Contains(q));

        return await query
            .OrderBy(s => s.Name)
            .Take(Math.Clamp(limit, 5, 200))
            .Select(s => new SensorDto
            {
                Id = s.Id,
                Name = s.Name,
                MacAddress = s.MacAddress,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Status = s.Status,
                LastSeenAt = s.LastSeenAt
            })
            .ToListAsync();
    }
}