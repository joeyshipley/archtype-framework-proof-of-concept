using FluentValidation;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Application.Todo.DeleteTodo;

public class DeleteTodoResponse : IResponse
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteTodoRequest : IRequest
{
    public long Id { get; set; }
}

public class DeleteTodoRequestValidator : AbstractValidator<DeleteTodoRequest>
{
    public DeleteTodoRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Todo ID must be greater than 0.");
    }
}
