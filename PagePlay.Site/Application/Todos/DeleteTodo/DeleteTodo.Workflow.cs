using FluentValidation;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.DeleteTodo;

public class DeleteTodoWorkflow(
    IValidator<DeleteTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<DeleteTodoRequest, DeleteTodoResponse>
{
    public async Task<IApplicationResult<DeleteTodoResponse>> Perform(DeleteTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return response("Todo not found.");

        if (!userOwnsTodo(todo))
            return response("You do not have permission to delete this todo.");

        await deleteTodo(todo);

        return response(todo.Id);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(DeleteTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _todoRepository.GetById(id);

    private bool userOwnsTodo(Todo todo) =>
        todo.UserId == _authContext.UserId;

    private async Task deleteTodo(Todo todo)
    {
        await _todoRepository.Delete(todo);
        await _todoRepository.SaveChanges();
    }

    private IApplicationResult<DeleteTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<DeleteTodoResponse>.Fail(validationResult);

    private IApplicationResult<DeleteTodoResponse> response(string errorMessage) =>
        ApplicationResult<DeleteTodoResponse>.Fail(errorMessage);

    private IApplicationResult<DeleteTodoResponse> response(long todoId) =>
        ApplicationResult<DeleteTodoResponse>.Succeed(
            new DeleteTodoResponse
            {
                Id = todoId,
                Message = "Todo deleted successfully."
            }
        );
}
