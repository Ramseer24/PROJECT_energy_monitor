using System;

namespace PowerMonitor.API.DTOs
{
    /// <summary>
    /// DTO для прийому нових зчитувань від сенсорів (IoT-модуль)
    /// </summary>
    public record ReadingDto(
        int GeneratorId,
        double Voltage,
        double Current,
        double Temperature,
        int Rpm);

    /// <summary>
    /// DTO для створення генератора
    /// </summary>
    public record GeneratorCreateDto(
        string Name,
        string Type,
        double MaxPowerOutput);

    /// <summary>
    /// DTO для відображення сповіщення (корисний для адмін-панелі)
    /// </summary>
    public record AlertDto(
        int Id,
        string GeneratorName,
        string Message,
        DateTime CreatedAt,
        bool Acknowledged,
        DateTime? AcknowledgedAt);
}