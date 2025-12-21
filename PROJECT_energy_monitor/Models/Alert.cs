using System.Text.Json.Serialization;

namespace PowerMonitor.API.Models;

public class Alert
{
    public int AlertId { get; set; }
    public int ThresholdId { get; set; }
    public long ReadingId { get; set; }
    public string? Message { get; set; }
    public bool Acknowledged { get; set; } = false;
    public int? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Threshold? Threshold { get; set; }

    [JsonIgnore]
    public SensorReading? Reading { get; set; }
}