using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using Todo = PagePlay.Site.Application.Todos.Models.Todo;

namespace PagePlay.Site.Application.Todos.Performers.CreateTodo;

public class CreateTodoPerformer(
    IValidator<CreateTodoRequest> _validator,
    ICurrentUserContext currentUserContext,
    IRepository _repository
) : PerformerBase<CreateTodoRequest, CreateTodoResponse>, IPerformer<CreateTodoRequest, CreateTodoResponse>
{
    public async Task<IApplicationResult<CreateTodoResponse>> Perform(CreateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await createTodo(request);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(CreateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> createTodo(CreateTodoRequest request)
    {
        var todo = Todo.Create(currentUserContext.UserId.Value, request.Title);
        await _repository.Add<Todo>(todo);
        await _repository.SaveChanges();
        return todo;
    }

    private CreateTodoResponse buildResponse(Todo todo) =>
        new CreateTodoResponse
        {
            CreatedId = todo.Id
        };
}
