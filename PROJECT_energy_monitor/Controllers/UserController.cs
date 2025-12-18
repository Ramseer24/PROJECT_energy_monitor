using Microsoft.AspNetCore.Mvc;
using PowerMonitor.API.Models;
using PowerMonitor.API.DTOs;
using PowerMonitor.API.Repositories;

namespace PowerMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class UserController : ControllerBase
{
    private readonly IGenericRepository<User> _repo;

    public UserController(IGenericRepository<User> repo) => _repo = repo;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await _repo.GetByIdAsync(id) is User u ? Ok(u) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = dto.PasswordHash, // У реальному проекті хешувати!
            FullName = dto.FullName,
            Role = dto.Role
        };

        await _repo.AddAsync(user);
        await _repo.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] User updated)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();

        item.Username = updated.Username;
        item.PasswordHash = updated.PasswordHash;
        item.FullName = updated.FullName;
        item.Role = updated.Role;

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