using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoWorkflow(
    IValidator<ToggleTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<ToggleTodoRequest, ToggleTodoResponse>
{
    public async Task<IApplicationResult<ToggleTodoResponse>> Perform(ToggleTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return response("Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId))
            return response("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        return response(todo);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(ToggleTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _todoRepository.GetByIdForUpdate(id);

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _todoRepository.SaveChanges();

    private IApplicationResult<ToggleTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<ToggleTodoResponse>.Fail(validationResult);

    private IApplicationResult<ToggleTodoResponse> response(string errorMessage) =>
        ApplicationResult<ToggleTodoResponse>.Fail(errorMessage);

    private IApplicationResult<ToggleTodoResponse> response(Todo todo) =>
        ApplicationResult<ToggleTodoResponse>.Succeed(
            new ToggleTodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            }
        );
}
