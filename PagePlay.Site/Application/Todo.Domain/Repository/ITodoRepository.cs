using PagePlay.Site.Application.Todo.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todo.Domain.Repository;

public interface ITodoRepository : IRepository<Models.Todo>
{
    Task<Models.Todo> GetById(long id);
    Task<List<Models.Todo>> GetAll();
    Task<List<Models.Todo>> GetCompleted();
    Task<List<Models.Todo>> GetIncomplete();
}
