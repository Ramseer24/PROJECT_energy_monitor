using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]s")] // → /api/alerts
public class AlertController : ControllerBase
{
    private readonly IGenericRepository<Alert> _repo;

    public AlertController(IGenericRepository<Alert> repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _repo.GetByIdAsync(id) is Alert a ? Ok(a) : NotFound();

    // Підтвердження сповіщення адміністратором
    [HttpPatch("{id:int}/resolve")]
    public async Task<IActionResult> Resolve(int id, [FromBody] int userId)
    {
        var alert = await _repo.GetByIdAsync(id);
        if (alert == null) return NotFound();

        alert.Acknowledged = true;
        alert.AcknowledgedBy = userId;
        alert.AcknowledgedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(alert);
        await _repo.SaveChangesAsync();

        return Ok(alert);
    }

    // DELETE (якщо потрібно видаляти старі сповіщення)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var alert = await _repo.GetByIdAsync(id);
        if (alert == null) return NotFound();

        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
        return NoContent();
    }
}