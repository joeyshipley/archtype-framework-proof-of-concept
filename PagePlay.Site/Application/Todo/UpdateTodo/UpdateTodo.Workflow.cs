using FluentValidation;
using PagePlay.Site.Application.Todo.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todo.UpdateTodo;

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

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return response("Todo not found.");

        if (!userOwnsTodo(todo))
            return response("You do not have permission to modify this todo.");

        updateTodoTitle(todo, request.Title);
        await saveTodo(todo);

        return response(todo);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(UpdateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Domain.Models.Todo> getTodoById(long id) =>
        await _todoRepository.GetByIdUntracked(id);

    private bool userOwnsTodo(Domain.Models.Todo todo) =>
        todo.UserId == _authContext.UserId;

    private void updateTodoTitle(Domain.Models.Todo todo, string title) =>
        todo.UpdateTitle(title);

    private async Task saveTodo(Domain.Models.Todo todo) =>
        await _todoRepository.SaveChanges();

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
