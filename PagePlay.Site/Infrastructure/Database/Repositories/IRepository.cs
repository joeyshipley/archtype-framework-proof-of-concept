using PagePlay.Site.Infrastructure.Database.Specifications;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

public interface IRepository
{
    Task<T> Get<T>(Specification<T> spec) where T : class, IEntity;
    Task<T> GetForUpdate<T>(Specification<T> spec) where T : class, IEntity;
    Task<List<T>> List<T>(Specification<T> spec) where T : class, IEntity;
    Task<bool> Exists<T>(Specification<T> spec) where T : class, IEntity;
    Task<T> Add<T>(T entity) where T : class, IEntity;
    Task Update<T>(T entity) where T : class, IEntity;
    Task Delete<T>(T entity) where T : class, IEntity;
    Task SaveChanges();
    ITransactionScope BeginTransactionScope();
}

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task CompleteTransaction();
}
