using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.CreateTodo;

public class CreateTodoWorkflow(
    IValidator<CreateTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<CreateTodoRequest, CreateTodoResponse>
{
    public async Task<IApplicationResult<CreateTodoResponse>> Perform(CreateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todo = createTodo(request);
        await saveTodo(todo);

        return response(todo);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(CreateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private Todo createTodo(CreateTodoRequest request) =>
        Todo.Create(_authContext.UserId, request.Title);

    private async Task saveTodo(Todo todo)
    {
        await _todoRepository.Add(todo);
        await _todoRepository.SaveChanges();
    }

    private IApplicationResult<CreateTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<CreateTodoResponse>.Fail(validationResult);

    private IApplicationResult<CreateTodoResponse> response(Todo todo) =>
        ApplicationResult<CreateTodoResponse>.Succeed(
            new CreateTodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            }
        );
}
