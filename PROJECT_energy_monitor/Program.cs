using PowerMonitor.API.Data;
using PowerMonitor.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// === Реєстрація DbContext з хмарною базою ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "postgresql://project_energy_monitor_user:1MPealRnWRxYeJJgW3K5EdPxBe4U8Yg7@dpg-d4utfejuibfs73f6tif0-a.frankfurt-postgres.render.com/project_energy_monitor"));

// === Репозиторій ===
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// === Контролери ===
builder.Services.AddControllers();

// === Swagger (виправлено синтаксис OpenApiInfo) ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PowerMonitor API",  // Виправлено: прибрано кому та зайвий "Title"
        Version = "v1",
        Description = "API для моніторингу генерації електроенергії (тестовий проект з імітацією даних від сенсорів)"
    });
});

// === Kestrel для Render ===
builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
    options.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

// === Swagger на корені ===
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMonitor API v1");
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();