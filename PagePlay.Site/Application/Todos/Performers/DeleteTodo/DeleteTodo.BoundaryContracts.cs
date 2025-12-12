using FluentValidation;
using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Application.Todos.Performers.DeleteTodo;

public class DeleteTodoResponse : IPerformerResponse
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteTodoRequest : IPerformerRequest
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
