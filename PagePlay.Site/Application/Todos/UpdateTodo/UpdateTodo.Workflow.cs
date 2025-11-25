using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.UpdateTodo;

public class UpdateTodoWorkflow(
    IValidator<UpdateTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<UpdateTodoRequest, UpdateTodoResponse>
{
    public async Task<IApplicationResult<UpdateTodoResponse>> Perform(UpdateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var (todo, errorMessage) = await getTodo(request.Id);
        if (!string.IsNullOrEmpty(errorMessage))
            return response(errorMessage);

        await changeTitle(todo, request.Title);

        return response(todo);
    }

    private async Task<(Todo todo, string errorMessage)> getTodo(long id)
    {
        var todo = await _todoRepository.GetByIdForUpdate(id);
        if (todo == null)
            return (null, "Todo not found.");

        if (!userOwnsTodo(todo))
            return (null, "You do not have permission to modify this todo.");

        return (todo, null);
    }
    
    private async Task<FluentValidation.Results.ValidationResult> validate(UpdateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private bool userOwnsTodo(Todo todo) =>
        todo.UserId == _authContext.UserId;

    private async Task changeTitle(Todo todo, string title)
    { 
        todo.UpdateTitle(title);
        await _todoRepository.SaveChanges();
    }

    private IApplicationResult<UpdateTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<UpdateTodoResponse>.Fail(validationResult);

    private IApplicationResult<UpdateTodoResponse> response(string errorMessage) =>
        ApplicationResult<UpdateTodoResponse>.Fail(errorMessage);

    private IApplicationResult<UpdateTodoResponse> response(Domain.Models.Todo todo) =>
        ApplicationResult<UpdateTodoResponse>.Succeed(
            new UpdateTodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                UpdatedAt = todo.UpdatedAt
            }
        );
}
