
using BackendAspNetCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendAspNetCore.Data.Configuration;

public class UserEntityConfig : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        // Single Index
        builder.HasIndex(s => s.MacAddress).IsUnique();
        builder.HasIndex(s => s.SerialNumber).IsUnique();

        // Composite Index
        builder.HasIndex(s => new { s.MacAddress, s.SerialNumber }).IsUnique();
        builder.HasIndex(s => new { s.Latitude, s.Longitude });
    }
}