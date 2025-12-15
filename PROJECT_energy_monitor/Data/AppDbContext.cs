using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Models;

namespace PowerMonitor.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Generator> Generators { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв'язку Generator -> SensorReading
            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Readings)
                .WithOne(r => r.Generator)
                .HasForeignKey(r => r.GeneratorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Налаштування зв'язку Generator -> Alert
            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Alerts)
                .WithOne(a => a.Generator)
                .HasForeignKey(a => a.GeneratorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Додатково: індекси для швидких запитів (рекомендовано для реального часу)
            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => r.Timestamp);

            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => new { r.GeneratorId, r.Timestamp });
        }
    }
}