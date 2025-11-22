using PagePlay.Site.Infrastructure.Database.Specifications;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetAsync(Specification<T> spec);
    Task<List<T>> ListAsync(Specification<T> spec);
    Task<bool> AnyAsync(Specification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}
