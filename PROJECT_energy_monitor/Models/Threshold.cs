using System.Text.Json.Serialization;

namespace PowerMonitor.API.Models;

public class Threshold
{
    public int ThresholdId { get; set; }
    public int SensorId { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public string AlertMessage { get; set; } = "Відхилення від норми!";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public Sensor? Sensor { get; set; }
}