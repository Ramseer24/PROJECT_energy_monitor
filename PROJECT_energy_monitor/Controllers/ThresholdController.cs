using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class ThresholdController : ControllerBase
{
    private readonly IGenericRepository<Threshold> _repo;

    public ThresholdController(IGenericRepository<Threshold> repo) => _repo = repo;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _repo.GetByIdAsync(id) is Threshold t ? Ok(t) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Threshold threshold)
    {
        await _repo.AddAsync(threshold);
        await _repo.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = threshold.ThresholdId}, threshold);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Threshold updated)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();

        // У методі Update:
        item.SensorId = updated.SensorId;                                         // решта коду без змін     // Один раз
        item.MinValue = updated.MinValue;
        item.MaxValue = updated.MaxValue;
        item.AlertMessage = updated.AlertMessage;
        item.IsActive = updated.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        await _repo.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();

        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
        return NoContent();
    }
}