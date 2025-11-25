using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Repositories;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public interface IUserRepository : IRepository<User>
{
    // EmailExists provides semantic value - it's clear and concise
    Task<bool> EmailExists(string email);
}
