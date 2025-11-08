
using BackendAspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAspNetCore.Data;

public static class SensorSeed
{
    // Call this once in Program.cs during dev if you want demo data.
    public static async Task EnsureSeedAsync(IServiceProvider sp, int total = 10)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (await db.Sensors.AnyAsync()) return;

        var rnd = new Random(42);

        // Rough NZ bounds
        var minLat = -47.0; var maxLat = -34.0;
        var minLng = 166.0; var maxLng = 179.0;

        var items = new List<Sensor>();
        for (int i = 0; i < total; i++)
        {
            var lat = minLat + rnd.NextDouble() * (maxLat - minLat);
            var lng = minLng + rnd.NextDouble() * (maxLng - minLng);
            items.Add(new Sensor
            {
                Id = Guid.NewGuid(),
                Name = $"YABBY-M1-{i:D5}",
                MacAddress = $"00-11-22-33-{i % 256:X2}-{(i * 7) % 256:X2}",
                SerialNumber = $"SN-{100000 + i}",
                Temperature = Math.Round(rnd.NextDouble() * 30 + 5, 1),
                Humidity = Math.Round(rnd.NextDouble() * 70 + 20, 1),
                Latitude = lat,
                Longitude = lng,
                Status = (i % 3 == 0) ? "stopped" : "moving",
                LastSeenAt = DateTimeOffset.UtcNow.AddMinutes(-rnd.Next(0, 60 * 24)),
                LastUpdatedUtc = DateTimeOffset.UtcNow.AddMinutes(-rnd.Next(0, 60 * 24))
            });
        }

        await db.AddRangeAsync(items);
        await db.SaveChangesAsync();
    }
}
