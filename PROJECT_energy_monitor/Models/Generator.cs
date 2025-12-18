using System.ComponentModel.DataAnnotations;

namespace PowerMonitor.API.Models
{
    public class Generator
    {
        [Key]
        public int GeneratorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double MaxPowerOutput { get; set; }

        public List<SensorReading> Readings { get; set; } = new();
        public List<Alert> Alerts { get; set; } = new();
    }
}