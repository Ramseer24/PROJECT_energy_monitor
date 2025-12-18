using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class GeneratorController : ControllerBase
{
    private readonly IGenericRepository<Generator> _repo;

    public GeneratorController(IGenericRepository<Generator> repo) => _repo = repo;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _repo.GetByIdAsync(id) is Generator g ? Ok(g) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Generator generator)
    {
        await _repo.AddAsync(generator);
        await _repo.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = generator.GeneratorId}, generator);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Generator updated)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();

        item.Name = updated.Name;
        // Додай інші поля за потребою

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