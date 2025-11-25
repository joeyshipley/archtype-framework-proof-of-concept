using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todos.Domain.Repository;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(IDbContextFactory<AppDbContext> _)
        : base(_) {}

    public async Task<Todo> GetById(long id)
    {
        return await Get(TodoSpecifications.ById(id));
    }

    public async Task<Todo> GetByIdForUpdate(long id)
    {
        return await GetForUpdate(TodoSpecifications.ById(id));
    }

    public async Task<List<Todo>> GetAll()
    {
        return await List(TodoSpecifications.All());
    }

    public async Task<List<Todo>> GetCompleted()
    {
        return await List(TodoSpecifications.Completed());
    }

    public async Task<List<Todo>> GetIncomplete()
    {
        return await List(TodoSpecifications.Incomplete());
    }

    public async Task<List<Todo>> GetByUserId(long userId)
    {
        return await List(TodoSpecifications.ByUserId(userId));
    }

    public async Task<List<Todo>> GetCompletedByUserId(long userId)
    {
        return await List(TodoSpecifications.CompletedByUserId(userId));
    }

    public async Task<List<Todo>> GetIncompleteByUserId(long userId)
    {
        return await List(TodoSpecifications.IncompleteByUserId(userId));
    }
}
