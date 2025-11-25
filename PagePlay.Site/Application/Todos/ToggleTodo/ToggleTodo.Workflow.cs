using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoWorkflow(
    IValidator<ToggleTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : WorkflowBase<ToggleTodoRequest, ToggleTodoResponse>, IWorkflow<ToggleTodoRequest, ToggleTodoResponse>
{
    public async Task<IApplicationResult<ToggleTodoResponse>> Perform(ToggleTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId))
            return Fail("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(ToggleTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _todoRepository.GetByIdForUpdate(id);

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _todoRepository.SaveChanges();

    private ToggleTodoResponse buildResponse(Todo todo) =>
        new ToggleTodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
}
