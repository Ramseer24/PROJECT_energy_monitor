using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.DTOs;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/readings")]  // ЯВНО ВКАЗАНО, щоб уникнути плутанини
public class SensorReadingController : ControllerBase
{
    private readonly IGenericRepository<SensorReading> _readingRepo;
    private readonly IGenericRepository<Generator> _generatorRepo;
    private readonly IGenericRepository<Threshold> _thresholdRepo;
    private readonly IGenericRepository<Alert> _alertRepo;

    public SensorReadingController(
        IGenericRepository<SensorReading> readingRepo,
        IGenericRepository<Generator> generatorRepo,
        IGenericRepository<Threshold> thresholdRepo,
        IGenericRepository<Alert> alertRepo)
    {
        _readingRepo = readingRepo;
        _generatorRepo = generatorRepo;
        _thresholdRepo = thresholdRepo;
        _alertRepo = alertRepo;
    }

    // POST: імітація надходження даних від сенсорів (IoT)
    // У конструкторі та полях — без змін

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReadingDto dto)
    {
        var generator = await _generatorRepo.GetByIdAsync(dto.GeneratorId);
        if (generator == null) return BadRequest("Generator not found");

        var reading = new SensorReading
        {
            GeneratorId = dto.GeneratorId,
            Voltage = dto.Voltage,
            Current = dto.Current,
            Power = dto.Voltage * dto.Current,
            Temperature = dto.Temperature,
            Rpm = dto.Rpm,
            Timestamp = DateTime.UtcNow
        };

        await _readingRepo.AddAsync(reading);
        await _readingRepo.SaveChangesAsync();

        // === Перевірка порогів ===
        var thresholds = await _thresholdRepo.GetAllAsync();
        var relevantThresholds = thresholds.Where(t => t.IsActive &&
            (t.GeneratorId == null || t.GeneratorId == 0 || t.GeneratorId == dto.GeneratorId));

        foreach (var thresh in relevantThresholds)
        {
            bool isAlert = false;
            string param = "";

            if (thresh.MinValue.HasValue || thresh.MaxValue.HasValue)
            {
                // Перевірка температури
                if (thresh.MinValue.HasValue && reading.Temperature < thresh.MinValue.Value)
                {
                    isAlert = true;
                    param = "температури (занадто низька)";
                }
                if (thresh.MaxValue.HasValue && reading.Temperature > thresh.MaxValue.Value)
                {
                    isAlert = true;
                    param = "температури (занадто висока)";
                }

                // Додаткові перевірки (можна розширити)
                if (thresh.MinValue.HasValue && reading.Voltage < thresh.MinValue.Value)
                {
                    isAlert = true;
                    param = "напруги (занадто низька)";
                }
                if (thresh.MaxValue.HasValue && reading.Voltage > thresh.MaxValue.Value)
                {
                    isAlert = true;
                    param = "напруги (занадто висока)";
                }

                if (isAlert)
                {
                    var alert = new Alert
                    {
                        GeneratorId = dto.GeneratorId,
                        ThresholdId = thresh.ThresholdId,
                        Message = thresh.AlertMessage ?? $"Відхилення {param}!",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _alertRepo.AddAsync(alert);
                    await _alertRepo.SaveChangesAsync();
                }
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = reading.SensorReadingId }, reading);
    }

    // Стандартний CRUD (для адмін-панелі)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _readingRepo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _readingRepo.GetByIdAsync(id) is SensorReading r ? Ok(r) : NotFound();
}