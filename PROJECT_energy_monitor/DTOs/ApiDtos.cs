namespace PowerMonitor.API.DTOs
{
    // Record типи дуже зручні для DTO
    public record ReadingDto(int GeneratorId, double Voltage, double Current, double Temperature, int Rpm);
    public record AlertDto(int Id, string GeneratorName, string Message, DateTime Time);
}