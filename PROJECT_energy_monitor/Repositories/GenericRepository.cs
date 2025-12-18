using Microsoft.EntityFrameworkCore;
using PowerMonitor.API.Data;

namespace PowerMonitor.API.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    // ВИПРАВЛЕНО: правильне оновлення з відстеженням
    public async Task UpdateAsync(T entity)
    {
        // Якщо сутність вже відстежується — просто оновлюємо
        _context.Entry(entity).State = EntityState.Modified;
        // Або альтернативно: _dbSet.Update(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}