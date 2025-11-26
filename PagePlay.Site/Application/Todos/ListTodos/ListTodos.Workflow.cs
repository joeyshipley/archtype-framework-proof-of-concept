using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ListTodos;

public class ListTodosWorkflow(
    IValidator<ListTodosWorkflowRequest> _validator,
    IAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<ListTodosWorkflowRequest, ListTodosWorkflowResponse>, IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse>
{
    public async Task<IApplicationResult<ListTodosWorkflowResponse>> Perform(ListTodosWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todos = await getTodosByUserId();

        return Succeed(buildResponse(todos));
    }

    private async Task<ValidationResult> validate(ListTodosWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<List<Todo>> getTodosByUserId() =>
        await _repository.List(Todo.ByUserId(_authContext.UserId.Value));

    private ListTodosWorkflowResponse buildResponse(List<Todo> todos) =>
        new ListTodosWorkflowResponse
        {
            Todos = todos
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .Select(TodoListEntry.FromTodo)
                .ToList()
        };
}
