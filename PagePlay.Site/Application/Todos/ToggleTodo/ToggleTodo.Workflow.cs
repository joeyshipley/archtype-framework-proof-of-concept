using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoWorkflow(
    IValidator<ToggleTodoWorkflowRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse>, IWorkflow<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse>
{
    public async Task<IApplicationResult<ToggleTodoWorkflowResponse>> Perform(ToggleTodoWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(workflowRequest.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId))
            return Fail("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        var todos = await getTodosByUserId();
        return Succeed(buildResponse(todos));
    }

    private async Task<ValidationResult> validate(ToggleTodoWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.GetForUpdate(Todo.ById(id));

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _repository.SaveChanges();

    private async Task<List<Todo>> getTodosByUserId() =>
        await _repository.List<Todo>(Todo.ByUserId(_authContext.UserId));

    private ToggleTodoWorkflowResponse buildResponse(List<Todo> todos) =>
        new ToggleTodoWorkflowResponse
        {
            Todos = todos
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .Select(TodoListEntry.FromTodo)
                .ToList()
        };
}
