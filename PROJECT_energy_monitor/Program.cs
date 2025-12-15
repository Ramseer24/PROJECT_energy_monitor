using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Data;
using PowerMonitor.API.Models;
using PowerMonitor.API.DTOs;
using PowerMonitor.API.Repositories; // ← ОБОВ’ЯЗКОВО додати цей using

var builder = WebApplication.CreateBuilder(args);

// Підключення до БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Реєстрація репозиторіїв ===
builder.Services.AddScoped<IGenericRepository<Generator>, GenericRepository<Generator>>();
builder.Services.AddScoped<IGenericRepository<SensorReading>, GenericRepository<SensorReading>>();
builder.Services.AddScoped<IGenericRepository<Alert>, GenericRepository<Alert>>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ======================
// Приклади endpoints з репозиторіями (тепер точно працюватимуть)
// ======================

// Отримати всі генератори
app.MapGet("/api/generators", async (IGenericRepository<Generator> repo) =>
{
    var generators = await repo.GetAllAsync();
    var dtos = generators.Select(g => new GeneratorDto(g.Id, g.Name, g.Type, g.MaxPowerOutput));
    return Results.Ok(dtos);
});

// Додати нове зчитування від IoT
app.MapPost("/api/readings", async (
    ReadingDto dto,
    IGenericRepository<SensorReading> readingRepo,
    IGenericRepository<Generator> generatorRepo) =>
{
    var generator = await generatorRepo.GetByIdAsync(dto.GeneratorId);
    if (generator == null) return Results.BadRequest("Generator not found");

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

    await readingRepo.AddAsync(reading);
    await readingRepo.SaveChangesAsync(); // зберігаємо зміни

    return Results.Created($"/api/readings", dto);
});

// Отримати всі сповіщення
app.MapGet("/api/alerts", async (IGenericRepository<Alert> repo) =>
{
    var alerts = await repo.GetAllAsync();
    var dtos = alerts.Select(a => new AlertDto(
        a.Id,
        a.Generator?.Name ?? "Unknown",
        a.Message,
        a.CreatedAt));
    return Results.Ok(dtos);
});

app.Run();