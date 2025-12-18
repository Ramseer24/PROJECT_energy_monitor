using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMonitor.API.Models
{
    public class SensorReading
    {
        [Key]
        public int SensorReadingId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double Power { get; set; }
        public double Temperature { get; set; }
        public int Rpm { get; set; }

        [ForeignKey("Generator")]
        public int GeneratorId { get; set; }
        public Generator? Generator { get; set; }
    }
}