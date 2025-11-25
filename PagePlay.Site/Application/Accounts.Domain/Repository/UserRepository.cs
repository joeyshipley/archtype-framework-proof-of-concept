using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory) {}

    public async Task<bool> EmailExists(string email)
    {
        return await Any(UserSpecifications.ByEmail(email));
    }
}
