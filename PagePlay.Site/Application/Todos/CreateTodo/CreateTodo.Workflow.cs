using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.CreateTodo;

public class CreateTodoWorkflow(
    IValidator<CreateTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<CreateTodoRequest, CreateTodoResponse>, IWorkflow<CreateTodoRequest, CreateTodoResponse>
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
        var todo = Todo.Create(_authContext.UserId, request.Title);
        await _repository.Add<Todo>(todo);
        await _repository.SaveChanges();
        return todo;
    }

    private CreateTodoResponse buildResponse(Todo todo) =>
        new CreateTodoResponse
        {
            Todo = TodoListEntry.FromTodo(todo)
        };
}
