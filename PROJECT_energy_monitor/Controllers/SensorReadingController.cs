using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.DTOs;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;
using PROJECT_energy_monitor.DTOs;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/readings")]
public class SensorReadingController : ControllerBase
{
    private readonly IGenericRepository<SensorReading> _readingRepo;
    private readonly IGenericRepository<Sensor> _sensorRepo;
    private readonly IGenericRepository<Threshold> _thresholdRepo;
    private readonly IGenericRepository<Alert> _alertRepo;

    public SensorReadingController(
        IGenericRepository<SensorReading> readingRepo,
        IGenericRepository<Sensor> sensorRepo,
        IGenericRepository<Threshold> thresholdRepo,
        IGenericRepository<Alert> alertRepo)
    {
        _readingRepo = readingRepo;
        _sensorRepo = sensorRepo;
        _thresholdRepo = thresholdRepo;
        _alertRepo = alertRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReadingsDto dto)
    {
        var sensor = await _sensorRepo.GetByIdAsync(dto.SensorId);
        if (sensor == null) return BadRequest("Sensor not found");

        var reading = new SensorReading
        {
            SensorId = dto.SensorId,
            Value = dto.Value,
            Timestamp = DateTime.UtcNow
        };

        await _readingRepo.AddAsync(reading);
        await _readingRepo.SaveChangesAsync();

        var thresholds = await _thresholdRepo.GetAllAsync();
        var activeThresholds = thresholds.Where(t => t.IsActive && t.SensorId == dto.SensorId);

        foreach (var t in activeThresholds)
        {
            if ((t.MinValue.HasValue && dto.Value < t.MinValue.Value) ||
                (t.MaxValue.HasValue && dto.Value > t.MaxValue.Value))
            {
                var alert = new Alert
                {
                    ThresholdId = t.ThresholdId,
                    ReadingId = reading.ReadingId,
                    Message = t.AlertMessage,
                    CreatedAt = DateTime.UtcNow
                };

                await _alertRepo.AddAsync(alert);
                await _alertRepo.SaveChangesAsync();
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = reading.ReadingId }, reading);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _readingRepo.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id) =>
        await _readingRepo.GetByIdAsync((int)id) is SensorReading r ? Ok(r) : NotFound();
}