using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todos.Domain.Repository;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory) {}
}
