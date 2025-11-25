using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todos.Domain.Repository;

public interface ITodoRepository : IRepository<Todo>
{
    // Empty for now - ready for truly special repository logic when needed
    // Use specifications through inherited IRepository<Todo> methods for queries
}
