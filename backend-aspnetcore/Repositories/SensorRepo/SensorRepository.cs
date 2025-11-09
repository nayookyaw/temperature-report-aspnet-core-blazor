
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

    public async Task<Sensor?> GetSensorByMacAddress(string macAddress)
    {
        return await _db.Sensors
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.MacAddress == macAddress);
    }
    
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

    // public async Task<IEnumerable<SensorDto>> GetAllSensor(CancellationToken ct = default)
    // {
    //     return await _db.Sensors
    //         .AsNoTracking()
    //         .AsSplitQuery()
    //         .OrderBy(s => s.MacAddress)
    //         .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
    //         .ToListAsync(ct);
    // }

    public async Task<(IEnumerable<SensorDto> Items, long TotalCount)> SearchSensorListAsync
    (
        string? searchText, int page, int pageSize, CancellationToken ct = default
    )
    {
        IQueryable<Sensor> query = _db.Sensors
            .AsNoTracking()
            .AsSplitQuery(); // to avoid Cartesian explosion

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var s = searchText.Trim();
            query = query.Where(x =>
                x.MacAddress.StartsWith(s) ||
                x.SerialNumber.StartsWith(s));
        }

        var total = await query.LongCountAsync(ct);
        var items = await query
            .OrderBy(s => s.MacAddress)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<SensorDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IEnumerable<SensorDto>> GetSensorsInViewportAsync
    (
        double minLng, double minLat,
        double maxLng, double maxLat,
        int zoom, int limit, string? search,
        CancellationToken ct = default
    )
    {
        IQueryable<Sensor> query = _db.Sensors
            .AsNoTracking()
            .Where(s =>
                s.Longitude >= minLng && s.Longitude <= maxLng &&
                s.Latitude >= minLat && s.Latitude <= maxLat);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();

            // ✅ Avoid ToLower() — use EF.Functions.Like for index-friendly search
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{s}%") ||
                EF.Functions.Like(x.MacAddress, $"%{s}%"));
        }

        // ✅ Explicit ordering required for pagination or Take
        query = query.OrderByDescending(s => s.LastSeenAt);

        if (limit > 0)
            query = query.Take(limit);

        // ✅ Project only needed fields — fast and lean
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
            })
            .ToListAsync(ct);
    }
}