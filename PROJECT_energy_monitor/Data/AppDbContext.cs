using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Models;

namespace PowerMonitor.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Generator> Generators { get; set; } = null!;
        public DbSet<SensorReading> SensorReadings { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;
        public DbSet<Threshold> Thresholds { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Зв'язки та індекси (без змін)
            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Readings)
                .WithOne(r => r.Generator)
                .HasForeignKey(r => r.GeneratorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Alerts)
                .WithOne(a => a.Generator)
                .HasForeignKey(a => a.GeneratorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => r.Timestamp);
        }
    }
}