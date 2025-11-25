using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.UpdateTodo;

public class UpdateTodoWorkflow(
    IValidator<UpdateTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<UpdateTodoRequest, UpdateTodoResponse>, IWorkflow<UpdateTodoRequest, UpdateTodoResponse>
{
    public async Task<IApplicationResult<UpdateTodoResponse>> Perform(UpdateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var (todo, errorMessage) = await getTodo(request.Id);
        if (!string.IsNullOrEmpty(errorMessage))
            return Fail(errorMessage);

        await changeTitle(todo, request.Title);

        return Succeed(buildResponse(todo));
    }

    private async Task<(Todo todo, string errorMessage)> getTodo(long id)
    {
        var todo = await _repository.GetForUpdate(Todo.ById(id));
        if (todo == null)
            return (null, "Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId))
            return (null, "You do not have permission to modify this todo.");

        return (todo, null);
    }

    private async Task<ValidationResult> validate(UpdateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task changeTitle(Todo todo, string title)
    {
        todo.UpdateTitle(title);
        await _repository.SaveChanges();
    }

    private UpdateTodoResponse buildResponse(Todo todo) =>
        new UpdateTodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            UpdatedAt = todo.UpdatedAt
        };
}
