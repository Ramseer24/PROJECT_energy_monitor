using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Data;
using PowerMonitor.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери та Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found. Set DefaultConnection or DATABASE_URL environment variable.");
}

if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
    || connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
{
    var uri = new Uri(connectionString);
    var username = Uri.UnescapeDataString(uri.UserInfo.Split(':')[0]);
    var password = uri.UserInfo.Split(':').Length > 1
        ? Uri.UnescapeDataString(uri.UserInfo.Split(':')[1])
        : "";
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');

    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSLMode=Require";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Реєстрація репозиторіїв
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting. Environment: {Environment}", app.Environment.EnvironmentName);

if (connectionString.Contains("@"))
{
    var safeConnectionString = connectionString.Substring(0, connectionString.IndexOf("@") + 1) + "***";
    logger.LogInformation("Database connection string configured: {ConnectionString}", safeConnectionString);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMonitor API V1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/error", () => Results.Problem("An error occurred processing your request."))
    .ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .ExcludeFromDescription();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger_ = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply database migrations");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();