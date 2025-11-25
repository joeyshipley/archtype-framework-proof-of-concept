using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoWorkflow(
    IValidator<ToggleTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
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

        var todos = await getTodosByUserId();
        return Succeed(buildResponse(todos));
    }

    private async Task<ValidationResult> validate(ToggleTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.GetForUpdate(Todo.ById(id));

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _repository.SaveChanges();

    private async Task<List<Todo>> getTodosByUserId() =>
        await _repository.List<Todo>(Todo.ByUserId(_authContext.UserId));

    private ToggleTodoResponse buildResponse(List<Todo> todos) =>
        new ToggleTodoResponse
        {
            Todos = todos
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .Select(TodoListEntry.FromTodo)
                .ToList()
        };
}
