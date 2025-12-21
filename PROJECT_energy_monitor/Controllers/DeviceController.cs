using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/devices")]
public class DeviceController : ControllerBase
{
    private readonly IGenericRepository<Device> _repo;

    public DeviceController(IGenericRepository<Device> repo) => _repo = repo;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _repo.GetByIdAsync(id) is Device d ? Ok(d) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Device device)
    {
        await _repo.AddAsync(device);
        await _repo.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = device.DeviceId }, device);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Device updated)
    {
        var device = await _repo.GetByIdAsync(id);
        if (device == null) return NotFound();

        device.Name = updated.Name;
        device.Type = updated.Type;
        device.MaxPowerOutput = updated.MaxPowerOutput;
        device.IsActive = updated.IsActive;

        await _repo.UpdateAsync(device);
        await _repo.SaveChangesAsync();
        return Ok(device);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
        return NoContent();
    }
}