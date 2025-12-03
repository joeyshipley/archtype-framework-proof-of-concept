using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using Todo = PagePlay.Site.Application.Todos.Models.Todo;

namespace PagePlay.Site.Application.Todos.Workflows.CreateTodo;

public class CreateTodoWorkflow(
    IValidator<CreateTodoWorkflowRequest> _validator,
    ICurrentUserContext currentUserContext,
    IRepository _repository
) : WorkflowBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse>, IWorkflow<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse>
{
    public async Task<IApplicationResult<CreateTodoWorkflowResponse>> Perform(CreateTodoWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await createTodo(workflowRequest);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(CreateTodoWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<Todo> createTodo(CreateTodoWorkflowRequest workflowRequest)
    {
        var todo = Todo.Create(currentUserContext.UserId.Value, workflowRequest.Title);
        await _repository.Add<Todo>(todo);
        await _repository.SaveChanges();
        return todo;
    }

    private CreateTodoWorkflowResponse buildResponse(Todo todo) =>
        new CreateTodoWorkflowResponse
        {
            CreatedId = todo.Id
        };
}
