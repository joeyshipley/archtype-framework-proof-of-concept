using FluentValidation;
using PagePlay.Site.Application.Todo.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todo.ToggleTodo;

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

        if (!userOwnsTodo(todo))
            return response("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        return response(todo);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(ToggleTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Domain.Models.Todo> getTodoById(long id) =>
        await _todoRepository.GetById(id);

    private bool userOwnsTodo(Domain.Models.Todo todo) =>
        todo.UserId == _authContext.UserId;

    private void toggleTodo(Domain.Models.Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Domain.Models.Todo todo) =>
        await _todoRepository.SaveChanges();

    private IApplicationResult<ToggleTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<ToggleTodoResponse>.Fail(validationResult);

    private IApplicationResult<ToggleTodoResponse> response(string errorMessage) =>
        ApplicationResult<ToggleTodoResponse>.Fail(errorMessage);

    private IApplicationResult<ToggleTodoResponse> response(Domain.Models.Todo todo) =>
        ApplicationResult<ToggleTodoResponse>.Succeed(
            new ToggleTodoResponse
            {
                Id = todo.Id,
                IsCompleted = todo.IsCompleted,
                UpdatedAt = todo.UpdatedAt
            }
        );
}
