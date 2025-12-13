using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMonitor.API.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;

        [ForeignKey("Generator")]
        public int GeneratorId { get; set; }
        public Generator? Generator { get; set; }
    }
}