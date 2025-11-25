using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.DeleteTodo;

public class DeleteTodoWorkflow(
    IValidator<DeleteTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    IRepository _repository
) : WorkflowBase<DeleteTodoRequest, DeleteTodoResponse>, IWorkflow<DeleteTodoRequest, DeleteTodoResponse>
{
    public async Task<IApplicationResult<DeleteTodoResponse>> Perform(DeleteTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(_authContext.UserId))
            return Fail("You do not have permission to delete this todo.");

        await deleteTodo(todo);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(DeleteTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.Get<Todo>(Todo.ById(id));

    private async Task deleteTodo(Todo todo)
    {
        await _repository.Delete<Todo>(todo);
        await _repository.SaveChanges();
    }

    private DeleteTodoResponse buildResponse(Todo todo) =>
        new DeleteTodoResponse
        {
            Id = todo.Id,
            Message = "Todo deleted successfully."
        };
}
