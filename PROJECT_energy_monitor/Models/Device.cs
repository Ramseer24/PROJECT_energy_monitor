namespace PowerMonitor.API.Models;

public class Device
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? InstalledAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double? MaxPowerOutput { get; set; }

    public List<Sensor> Sensors { get; set; } = new();
}