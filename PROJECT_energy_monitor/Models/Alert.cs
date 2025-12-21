using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;  // ДОДАНО

namespace PowerMonitor.API.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;

        public bool Acknowledged { get; set; } = false;
        public int? AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }

        [ForeignKey("Generator")]
        public int GeneratorId { get; set; }

        [JsonIgnore]  // ДОДАНО – запобігає циклу
        public Generator? Generator { get; set; }

        public int? ThresholdId { get; set; }
    }
}