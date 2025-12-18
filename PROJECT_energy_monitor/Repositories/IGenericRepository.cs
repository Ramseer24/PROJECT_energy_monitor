namespace PowerMonitor.API.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);     // Новий асинхронний метод
    Task DeleteAsync(int id);       // Новий асинхронний метод (приймає id для зручності)
    Task SaveChangesAsync();
}