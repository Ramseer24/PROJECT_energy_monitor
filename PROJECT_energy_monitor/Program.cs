using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Data;
using PowerMonitor.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери та Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Підключення до PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація репозиторіїв
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Swagger доступний завжди
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMonitor API V1");
    c.RoutePrefix = "swagger";
});

// Автоматична міграція бази при старті (зручно для тестового/демо проекту)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate(); // Створює/оновлює БД
    }
    catch (Exception ex)
    {
        var logger = app.Logger;
        logger.LogError(ex, "Помилка застосування міграцій");
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();