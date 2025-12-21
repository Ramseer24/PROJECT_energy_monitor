using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Models;

namespace PowerMonitor.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<Sensor> Sensors { get; set; } = null!;
    public DbSet<SensorReading> SensorReadings { get; set; } = null!;
    public DbSet<Threshold> Thresholds { get; set; } = null!;
    public DbSet<Alert> Alerts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasMany(d => d.Sensors)
            .WithOne(s => s.Device)
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sensor>()
            .HasMany(s => s.Readings)
            .WithOne(r => r.Sensor)
            .HasForeignKey(r => r.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sensor>()
            .HasMany(s => s.Thresholds)
            .WithOne(t => t.Sensor)
            .HasForeignKey(t => t.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => new { r.SensorId, r.Timestamp });
    }
}