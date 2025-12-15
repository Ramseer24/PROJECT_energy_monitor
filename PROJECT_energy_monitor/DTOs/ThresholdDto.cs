namespace PowerMonitor.API.DTOs
{
    public record ThresholdDto(int SensorId, double? MinValue, double? MaxValue, string AlertMessage);
}