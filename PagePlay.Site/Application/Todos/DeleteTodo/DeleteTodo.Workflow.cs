using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.DeleteTodo;

public class DeleteTodoWorkflow(
    IValidator<DeleteTodoWorkflowRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse>, IWorkflow<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse>
{
    public async Task<IApplicationResult<DeleteTodoWorkflowResponse>> Perform(DeleteTodoWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(workflowRequest.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId.Value))
            return Fail("You do not have permission to delete this todo.");

        await deleteTodo(todo);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(DeleteTodoWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.Get(Todo.ById(id));

    private async Task deleteTodo(Todo todo)
    {
        await _repository.Delete(todo);
        await _repository.SaveChanges();
    }

    private DeleteTodoWorkflowResponse buildResponse(Todo todo) =>
        new DeleteTodoWorkflowResponse
        {
            Id = todo.Id,
            Message = "Todo deleted successfully."
        };
}
