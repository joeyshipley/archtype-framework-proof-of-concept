using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todos.Domain.Repository;

public interface ITodoRepository : IRepository<Todo>
{
    Task<Todo> GetById(long id);
    Task<Todo> GetByIdForUpdate(long id);
    Task<List<Todo>> GetAll();
    Task<List<Todo>> GetCompleted();
    Task<List<Todo>> GetIncomplete();
    Task<List<Todo>> GetByUserId(long userId);
    Task<List<Todo>> GetCompletedByUserId(long userId);
    Task<List<Todo>> GetIncompleteByUserId(long userId);
}
