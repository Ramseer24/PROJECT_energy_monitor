using System.Text.Json.Serialization;

namespace PowerMonitor.API.Models;

public class SensorReading
{
    public long ReadingId { get; set; }
    public int SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Sensor? Sensor { get; set; }
}