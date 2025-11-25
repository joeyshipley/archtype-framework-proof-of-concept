using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public interface IUserRepository : IRepository<User>
{
    // Empty for now - ready for truly special repository logic when needed
    // Use specifications through inherited IRepository<User> methods for queries
}
