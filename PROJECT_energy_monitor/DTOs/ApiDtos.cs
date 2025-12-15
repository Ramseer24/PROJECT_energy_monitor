// ApiDtos.cs (файл: DTOs/ApiDtos.cs)
namespace PowerMonitor.API.DTOs
{
    // DTO для нового зчитування від IoT-модуля
    public record ReadingDto(int GeneratorId, double Voltage, double Current, double Temperature, int Rpm)
    {
        // Додаткове поле для розрахунку потужності (P = U * I)
        public double Power => Voltage * Current;
    }

    // DTO для сповіщень
    public record AlertDto(int Id, string GeneratorName, string Message, DateTime Time);

    // DTO для генератора (спрощений)
    public record GeneratorDto(int Id, string Name, string Type, double MaxPowerOutput);

    // DTO для створення генератора
    public record GeneratorCreateDto(string Name, string Type, double MaxPowerOutput);
}