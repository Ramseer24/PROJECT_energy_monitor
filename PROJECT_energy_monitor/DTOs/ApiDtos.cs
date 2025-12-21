using System;

namespace PowerMonitor.API.DTOs
{
    public record ReadingDto(
        int GeneratorId,
        double Voltage,
        double Current,
        double Temperature,
        int Rpm)
    {
        internal int Value;
    }

    public record GeneratorCreateDto(
        string Name,
        string Type,
        double MaxPowerOutput);
    public record AlertDto(
        int Id,
        string GeneratorName,
        string Message,
        DateTime CreatedAt,
        bool Acknowledged,
        DateTime? AcknowledgedAt);
}