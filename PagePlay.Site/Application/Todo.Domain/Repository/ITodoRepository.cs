using PagePlay.Site.Application.Todo.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Todo.Domain.Repository;

public interface ITodoRepository : IRepository<Models.Todo>
{
    Task<Models.Todo> GetByIdUntracked(long id);
    Task<Models.Todo> GetById(long id);
    Task<List<Models.Todo>> GetAll();
    Task<List<Models.Todo>> GetCompleted();
    Task<List<Models.Todo>> GetIncomplete();
    Task<List<Models.Todo>> GetByUserId(long userId);
    Task<List<Models.Todo>> GetCompletedByUserId(long userId);
    Task<List<Models.Todo>> GetIncompleteByUserId(long userId);
}
