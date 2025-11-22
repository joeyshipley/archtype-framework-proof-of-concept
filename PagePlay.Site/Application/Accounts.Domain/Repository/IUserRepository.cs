using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmail(string email);
    Task<User> GetById(long id);
    Task<bool> EmailExists(string email);
}
