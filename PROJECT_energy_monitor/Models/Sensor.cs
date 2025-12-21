using System.Text.Json.Serialization;

namespace PowerMonitor.API.Models;

public class Sensor
{
    public int SensorId { get; set; }
    public int DeviceId { get; set; }
    public string SensorType { get; set; } = string.Empty;  // 'power', 'voltage', 'temperature', 'rpm'
    public string? Unit { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Device? Device { get; set; }

    public List<SensorReading> Readings { get; set; } = new();
    public List<Threshold> Thresholds { get; set; } = new();
}