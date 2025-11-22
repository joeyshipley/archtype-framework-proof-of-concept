using System.Linq.Expressions;

namespace PagePlay.Site.Infrastructure.Database.Specifications;

public class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Specification<T> Include(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
        return this;
    }
}
