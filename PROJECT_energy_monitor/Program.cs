using PowerMonitor.API.Data;
using PowerMonitor.API.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// === Репозиторій ===
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// === Контролери ===
builder.Services.AddControllers();

// === Swagger ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PowerMonitor API",
        Version = "v1",
        Description = "API для моніторингу генерації електроенергії (тестовий проект з імітацією даних від сенсорів)"
    });
});

var app = builder.Build();

// === Swagger на корені (доступний за https://project-energy-monitor.onrender.com/) ===
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;  // Swagger відкривається відразу на головній сторінці
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMonitor API v1");
});

// === HTTPS обов'язковий на Render ===
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// === Автовідкриття браузера видалено — працює тільки локально (в DEBUG) ===
// На хостингу (Render) це не потрібно, проект запускається автоматично