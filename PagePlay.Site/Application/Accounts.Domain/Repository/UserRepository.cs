using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDbContextFactory<AppDbContext> _) 
        : base(_) {}

    public async Task<User> GetByEmail(string email)
    {
        return await Get(UserSpecifications.ByEmail(email));
    }

    public async Task<User> GetByEmailForUpdate(string email)
    {
        return await GetForUpdate(UserSpecifications.ByEmail(email));
    }

    public async Task<User> GetById(long id)
    {
        return await Get(UserSpecifications.ById((int)id));
    }

    public async Task<User> GetByIdForUpdate(long id)
    {
        return await GetForUpdate(UserSpecifications.ById((int)id));
    }

    public async Task<bool> EmailExists(string email)
    {
        return await Any(UserSpecifications.ByEmail(email));
    }
}
