using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using Todo = PagePlay.Site.Application.Todos.Models.Todo;

namespace PagePlay.Site.Application.Todos.Workflows.ToggleTodo;

public class ToggleTodoWorkflow(
    IValidator<ToggleTodoWorkflowRequest> _validator,
    ICurrentUserContext currentUserContext,
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

        if (!todo.IsOwnedBy(currentUserContext.UserId.Value))
            return Fail("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        return Succeed(new ToggleTodoWorkflowResponse());
    }

    private async Task<ValidationResult> validate(ToggleTodoWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.GetForUpdate(Todo.ById(id));

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _repository.SaveChanges();
}
