using PagePlay.Site.Infrastructure.Database.Specifications;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<T> Get(Specification<T> spec);
    Task<List<T>> List(Specification<T> spec);
    Task<bool> Any(Specification<T> spec);
    Task<T> Add(T entity);
    Task Update(T entity);
    Task Delete(T entity);
    Task SaveChanges();
}
