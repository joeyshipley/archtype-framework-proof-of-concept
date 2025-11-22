using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Database.Specifications;

namespace PagePlay.Site.Infrastructure.Database.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> GetAsync(Specification<T> spec)
    {
        return await ApplySpecification(spec)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<T>> ListAsync(Specification<T> spec)
    {
        return await ApplySpecification(spec)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> AnyAsync(Specification<T> spec)
    {
        return await ApplySpecification(spec).AnyAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<T> ApplySpecification(Specification<T> spec)
    {
        var query = _context.Set<T>().Where(spec.Criteria);

        // Apply includes
        query = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
