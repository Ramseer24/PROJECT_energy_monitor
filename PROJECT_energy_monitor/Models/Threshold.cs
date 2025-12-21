using System.ComponentModel.DataAnnotations;

namespace PowerMonitor.API.Models
{
    public class Threshold
    {
        [Key]
        public int ThresholdId { get; set; }

        public int GeneratorId { get; set; }  // ЗМІНЕНО з SensorId → GeneratorId (логічніше)

        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public string AlertMessage { get; set; } = "Відхилення від норми";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}