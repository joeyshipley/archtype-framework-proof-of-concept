using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todo.ToggleTodo;

public class ToggleTodoResponse : IResponse
{
    public long Id { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ToggleTodoRequest : IRequest
{
    public long Id { get; set; }
}

public class ToggleTodoRequestValidator : AbstractValidator<ToggleTodoRequest>
{
    public ToggleTodoRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Todo ID must be greater than 0.");
    }
}
