using BackendAspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAspNetCore.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorLog> SensorLogs => Set<SensorLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}