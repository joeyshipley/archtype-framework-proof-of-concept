using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PagePlay.Site.Infrastructure.Data.Specifications;
using PagePlay.Site.Infrastructure.Core.Domain;

namespace PagePlay.Site.Infrastructure.Data.Repositories;

public class Repository : IRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private AppDbContext _context;

    public Repository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<AppDbContext> GetContext()
    {
        _context ??= await _contextFactory.CreateDbContextAsync();
        return _context;
    }

    public async Task<T> Get<T>(Specification<T> spec) where T : class, IEntity
    {
        var context = await GetContext();
        return await applySpecification(context, spec)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<T> GetForUpdate<T>(Specification<T> spec) where T : class, IEntity
    {
        var context = await GetContext();
        return await applySpecification(context, spec)
            .FirstOrDefaultAsync();
    }

    public async Task<List<T>> List<T>(Specification<T> spec) where T : class, IEntity
    {
        var context = await GetContext();
        return await applySpecification(context, spec)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Exists<T>(Specification<T> spec) where T : class, IEntity
    {
        var context = await GetContext();
        return await applySpecification(context, spec).AnyAsync();
    }

    public async Task<T> Add<T>(T entity) where T : class, IEntity
    {
        var context = await GetContext();
        await context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task Update<T>(T entity) where T : class, IEntity
    {
        var context = await GetContext();
        context.Set<T>().Update(entity);
    }

    public async Task Delete<T>(T entity) where T : class, IEntity
    {
        var context = await GetContext();
        context.Set<T>().Remove(entity);
    }

    public async Task SaveChanges()
    {
        var context = await GetContext();
        await context.SaveChangesAsync();
    }

    public ITransactionScope BeginTransactionScope()
    {
        var scope = new TransactionScope(this);
        // Start transaction immediately
        scope.Initialize().GetAwaiter().GetResult();
        return scope;
    }

    private IQueryable<T> applySpecification<T>(AppDbContext context, Specification<T> spec) where T : class, IEntity
    {
        var query = context.Set<T>().Where(spec.Criteria);

        query = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    internal class TransactionScope : ITransactionScope
    {
        private readonly Repository _repository;
        private IDbContextTransaction _transaction;
        private bool _completed = false;
        private bool _disposed = false;

        public TransactionScope(Repository repository)
        {
            _repository = repository;
        }

        internal async Task Initialize()
        {
            var context = await _repository.GetContext();
            _transaction = await context.Database.BeginTransactionAsync();
            // _logger.LogDebug("Transaction started");
        }

        public async Task CompleteTransaction()
        {
            if (_transaction == null)
            {
                // _logger.LogWarning("CompleteTransaction called but no transaction was started");
                return;
            }

            await _transaction.CommitAsync();
            _completed = true;
            // _logger.LogDebug("Transaction committed");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction != null && !_completed)
            {
                try
                {
                    _transaction.Rollback();
                    // _logger.LogWarning("Transaction rolled back - CompleteTransaction() was not called");
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Failed to rollback transaction");
                }
            }

            _transaction?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction != null && !_completed)
            {
                try
                {
                    await _transaction.RollbackAsync();
                    // _logger.LogWarning("Transaction rolled back - CompleteTransaction() was not called");
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Failed to rollback transaction");
                }
            }

            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
        }
    }
}
