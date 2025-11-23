using PagePlay.Site.Application.Todo.Domain.Models;
using PagePlay.Site.Infrastructure.Database.Specifications;

namespace PagePlay.Site.Application.Todo.Domain.Repository;

public static class TodoSpecifications
{
    public static Specification<Models.Todo> ById(long id)
    {
        return new Specification<Models.Todo>(t => t.Id == id);
    }

    public static Specification<Models.Todo> Completed()
    {
        return new Specification<Models.Todo>(t => t.IsCompleted == true);
    }

    public static Specification<Models.Todo> Incomplete()
    {
        return new Specification<Models.Todo>(t => t.IsCompleted == false);
    }

    public static Specification<Models.Todo> All()
    {
        return new Specification<Models.Todo>(t => true);
    }
}
