using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Database.Specifications;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> Get(Specification<T> spec)
    {
        return await applySpecification(spec)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<T>> List(Specification<T> spec)
    {
        return await applySpecification(spec)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Any(Specification<T> spec)
    {
        return await applySpecification(spec).AnyAsync();
    }

    public async Task<T> Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<T> applySpecification(Specification<T> spec)
    {
        var query = _context.Set<T>().Where(spec.Criteria);

        // Apply includes
        query = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
