using PowerMonitor.API.Data;
using PowerMonitor.API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери
builder.Services.AddControllers();

// Swagger завжди працює (навіть на Render у Production)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Підключення до PostgreSQL (з appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація репозиторіїв
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// === Swagger доступний завжди ===
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMonitor API V1");
    c.RoutePrefix = "swagger";  // Доступ за https://your-url.onrender.com/swagger
});

// === Автоматичне створення/оновлення бази при старті ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();  // Створює базу і таблиці, якщо їх немає
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при застосуванні міграцій бази даних");
        // На Render помилка буде видно в логах
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();