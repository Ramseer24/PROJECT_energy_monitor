using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]s")]  // → /api/generators
public class GeneratorController : ControllerBase
{
    private readonly IGenericRepository<Generator> _repo;

    public GeneratorController(IGenericRepository<Generator> repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.GetAllAsync();
        var result = items.Select(g => new
        {
            g.GeneratorId,
            g.Name,
            g.Type,
            g.MaxPowerOutput,
            ReadingsCount = g.Readings.Count,
            ActiveAlertsCount = g.Alerts.Count(a => !a.Acknowledged)
        });
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var g = await _repo.GetByIdAsync(id);
        if (g == null) return NotFound();

        return Ok(new
        {
            g.GeneratorId,
            g.Name,
            g.Type,
            g.MaxPowerOutput,
            ReadingsCount = g.Readings.Count,
            ActiveAlertsCount = g.Alerts.Count(a => !a.Acknowledged)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Generator generator)
    {
        if (generator == null) return BadRequest("Дані генератора не можуть бути порожніми.");

        await _repo.AddAsync(generator);
        await _repo.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = generator.GeneratorId }, generator);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Generator updated)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();

        item.Name = updated.Name;
        item.Type = updated.Type;
        item.MaxPowerOutput = updated.MaxPowerOutput;

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