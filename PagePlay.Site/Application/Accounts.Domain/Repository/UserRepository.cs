using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User> GetByEmail(string email)
    {
        return await Get(UserSpecifications.ByEmail(email));
    }

    public async Task<User> GetById(long id)
    {
        return await _context.Set<User>().FindAsync(id);
    }

    public async Task<bool> EmailExists(string email)
    {
        return await Any(UserSpecifications.ByEmail(email));
    }
}
