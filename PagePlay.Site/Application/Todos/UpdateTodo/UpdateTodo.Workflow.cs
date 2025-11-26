using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.UpdateTodo;

public class UpdateTodoWorkflow(
    IValidator<UpdateTodoWorkflowRequest> _validator,
    IAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<UpdateTodoWorkflowRequest, UpdateTodoWorkflowResponse>, IWorkflow<UpdateTodoWorkflowRequest, UpdateTodoWorkflowResponse>
{
    public async Task<IApplicationResult<UpdateTodoWorkflowResponse>> Perform(UpdateTodoWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var (todo, errorMessage) = await getTodo(workflowRequest.Id);
        if (!string.IsNullOrEmpty(errorMessage))
            return Fail(errorMessage);

        await changeTitle(todo, workflowRequest.Title);

        return Succeed(buildResponse(todo));
    }

    private async Task<(Todo todo, string errorMessage)> getTodo(long id)
    {
        var todo = await _repository.GetForUpdate(Todo.ById(id));
        if (todo == null)
            return (null, "Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId.Value))
            return (null, "You do not have permission to modify this todo.");

        return (todo, null);
    }

    private async Task<ValidationResult> validate(UpdateTodoWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task changeTitle(Todo todo, string title)
    {
        todo.UpdateTitle(title);
        await _repository.SaveChanges();
    }

    private UpdateTodoWorkflowResponse buildResponse(Todo todo) =>
        new UpdateTodoWorkflowResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            UpdatedAt = todo.UpdatedAt
        };
}
