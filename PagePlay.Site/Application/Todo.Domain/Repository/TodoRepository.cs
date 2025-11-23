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

    public async Task<Models.Todo> GetById(long id)
    {
        return await Get(TodoSpecifications.ById(id));
    }

    public async Task<List<Models.Todo>> GetAll()
    {
        return await List(TodoSpecifications.All());
    }

    public async Task<List<Models.Todo>> GetCompleted()
    {
        return await List(TodoSpecifications.Completed());
    }

    public async Task<List<Models.Todo>> GetIncomplete()
    {
        return await List(TodoSpecifications.Incomplete());
    }
}
