using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Specifications;

namespace PagePlay.Site.Application.Accounts.Domain.Repository;

public static class UserSpecifications
{
    public static Specification<User> ByEmail(string email)
    {
        return new Specification<User>(u => u.Email == email);
    }

    public static Specification<User> ById(int id)
    {
        return new Specification<User>(u => u.Id == id);
    }

    public static Specification<User> CreatedAfter(DateTime date)
    {
        return new Specification<User>(u => u.CreatedAt > date);
    }
}
