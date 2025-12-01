using PagePlay.Site.Infrastructure.Data.Specifications;

namespace PagePlay.Site.Application.Todos.Models;

public partial class Todo
{
    public static Specification<Todo> ById(long id)
    {
        return new Specification<Todo>(t => t.Id == id);
    }

    public static Specification<Todo> ByUserId(long userId)
    {
        return new Specification<Todo>(t => t.UserId == userId);
    }

    public static Specification<Todo> Completed()
    {
        return new Specification<Todo>(t => t.IsCompleted == true);
    }

    public static Specification<Todo> Incomplete()
    {
        return new Specification<Todo>(t => t.IsCompleted == false);
    }

    public static Specification<Todo> All()
    {
        return new Specification<Todo>(t => true);
    }

    public static Specification<Todo> CompletedByUserId(long userId)
    {
        return new Specification<Todo>(t => t.UserId == userId && t.IsCompleted == true);
    }

    public static Specification<Todo> IncompleteByUserId(long userId)
    {
        return new Specification<Todo>(t => t.UserId == userId && t.IsCompleted == false);
    }
}
