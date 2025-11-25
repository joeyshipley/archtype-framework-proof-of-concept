using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Todo.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todo.Domain.Repository;

public class TodoRepository : Repository<Models.Todo>, ITodoRepository
{
    public TodoRepository(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Models.Todo> GetByIdUntracked(long id)
    {
        return await GetUntracked(TodoSpecifications.ById(id));
    }

    public async Task<Models.Todo> GetById(long id)
    {
        return await GetTracked(TodoSpecifications.ById(id));
    }

    public async Task<List<Models.Todo>> GetAll()
    {
        return await ListUntracked(TodoSpecifications.All());
    }

    public async Task<List<Models.Todo>> GetCompleted()
    {
        return await ListUntracked(TodoSpecifications.Completed());
    }

    public async Task<List<Models.Todo>> GetIncomplete()
    {
        return await ListUntracked(TodoSpecifications.Incomplete());
    }

    public async Task<List<Models.Todo>> GetByUserId(long userId)
    {
        return await ListUntracked(TodoSpecifications.ByUserId(userId));
    }

    public async Task<List<Models.Todo>> GetCompletedByUserId(long userId)
    {
        return await ListUntracked(TodoSpecifications.CompletedByUserId(userId));
    }

    public async Task<List<Models.Todo>> GetIncompleteByUserId(long userId)
    {
        return await ListUntracked(TodoSpecifications.IncompleteByUserId(userId));
    }
}
