using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Database.Specifications;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

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

    private IQueryable<T> applySpecification<T>(AppDbContext context, Specification<T> spec) where T : class, IEntity
    {
        var query = context.Set<T>().Where(spec.Criteria);

        query = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
