using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Data;
using PowerMonitor.API.DTOs;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Підключення до бази даних
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація репозиторіїв
builder.Services.AddScoped<IGenericRepository<Generator>, GenericRepository<Generator>>();
builder.Services.AddScoped<IGenericRepository<SensorReading>, GenericRepository<SensorReading>>();
builder.Services.AddScoped<IGenericRepository<Alert>, GenericRepository<Alert>>();
builder.Services.AddScoped<IGenericRepository<Threshold>, GenericRepository<Threshold>>();
builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();

// Swagger для тестування API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ==================================================
// POST /api/readings — прийом даних від IoT-модуля
// ==================================================
app.MapPost("/api/readings", async (
    ReadingDto dto,
    IGenericRepository<SensorReading> readingRepo,
    IGenericRepository<Threshold> thresholdRepo,
    IGenericRepository<Alert> alertRepo) =>
{
    // Розрахунок потужності (математичний метод)
    double power = dto.Voltage * dto.Current;

    var reading = new SensorReading
    {
        GeneratorId = dto.GeneratorId,
        Voltage = dto.Voltage,
        Current = dto.Current,
        Power = power,
        Temperature = dto.Temperature,
        Rpm = dto.Rpm,
        Timestamp = DateTime.UtcNow
    };

    await readingRepo.AddAsync(reading);
    await readingRepo.SaveChangesAsync();

    // Бізнес-логіка: перевірка порогових значень
    // Для простоти тестування вважаємо, що пороги налаштовані для температури та потужності
    var allThresholds = await thresholdRepo.GetAllAsync();

    foreach (var threshold in allThresholds.Where(t => t.IsActive))
    {
        bool violation = false;

        // Перевірка по потужності
        if (threshold.MinValue.HasValue && power < threshold.MinValue.Value)
            violation = true;
        if (threshold.MaxValue.HasValue && power > threshold.MaxValue.Value)
            violation = true;

        // Перевірка по температурі (можна розширити для інших параметрів)
        if (threshold.MinValue.HasValue && dto.Temperature < threshold.MinValue.Value)
            violation = true;
        if (threshold.MaxValue.HasValue && dto.Temperature > threshold.MaxValue.Value)
            violation = true;

        if (violation)
        {
            var alert = new Alert
            {
                Message = threshold.AlertMessage ?? "Виявлено відхилення від норми",
                GeneratorId = dto.GeneratorId,
                ThresholdId = threshold.Id,
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            };

            await alertRepo.AddAsync(alert);
        }
    }

    await alertRepo.SaveChangesAsync();

    return Results.Created($"/api/readings/{reading.Id}", dto);
});

// ==================================================
// POST /api/thresholds — адміністрування: створення порогу
// ==================================================
app.MapPost("/api/thresholds", async (
    ThresholdDto dto,
    IGenericRepository<Threshold> thresholdRepo) =>
{
    var threshold = new Threshold
    {
        SensorId = dto.SensorId,
        MinValue = dto.MinValue,
        MaxValue = dto.MaxValue,
        AlertMessage = dto.AlertMessage,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    await thresholdRepo.AddAsync(threshold);
    await thresholdRepo.SaveChangesAsync();

    return Results.Created($"/api/thresholds/{threshold.Id}", dto);
});

// ==================================================
// PATCH /api/alerts/{id}/resolve — адміністрування: підтвердження сповіщення
// ==================================================
app.MapPatch("/api/alerts/{id:int}/resolve", async (
    int id,
    int userId,  // ID користувача, який підтверджує
    IGenericRepository<Alert> alertRepo) =>
{
    var alert = await alertRepo.GetByIdAsync(id);
    if (alert == null)
        return Results.NotFound();

    alert.IsResolved = true;
    alert.AcknowledgedBy = userId;
    alert.AcknowledgedAt = DateTime.UtcNow;

    await alertRepo.SaveChangesAsync();

    return Results.Ok(new { Message = "Сповіщення підтверджено", AlertId = id });
});

// Додаткові базові ендпоінти (для зручності тестування)
app.MapGet("/api/generators", async (IGenericRepository<Generator> repo) =>
    Results.Ok(await repo.GetAllAsync()));

app.MapGet("/api/alerts", async (IGenericRepository<Alert> repo) =>
    Results.Ok(await repo.GetAllAsync()));

app.Run();