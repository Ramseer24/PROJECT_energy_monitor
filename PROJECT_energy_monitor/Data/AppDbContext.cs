using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Models;

namespace PowerMonitor.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблиці бази даних
        public DbSet<Generator> Generators { get; set; } = null!;
        public DbSet<SensorReading> SensorReadings { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;
        public DbSet<Threshold> Thresholds { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Підключення до хмарної бази на Render (External URL з налаштувань)
                optionsBuilder.UseNpgsql(
                    "postgresql://project_energy_monitor_user:1MPealRnWRxYeJJgW3K5EdPxBe4U8Yg7@dpg-d4utfejuibfs73f6tif0-a.frankfurt-postgres.render.com/project_energy_monitor");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Налаштування зв'язків (каскадне видалення)
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

            // Індекс для швидких запитів за часом
            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => r.Timestamp);
        }
    }
}