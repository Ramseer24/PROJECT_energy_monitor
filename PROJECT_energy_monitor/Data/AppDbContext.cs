using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

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
            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Readings)
                .WithOne(r => r.Generator)
                .HasForeignKey(r => r.GeneratorId);

            modelBuilder.Entity<Generator>()
                .HasMany(g => g.Alerts)
                .WithOne(a => a.Generator)
                .HasForeignKey(a => a.GeneratorId);
        }
    }
}