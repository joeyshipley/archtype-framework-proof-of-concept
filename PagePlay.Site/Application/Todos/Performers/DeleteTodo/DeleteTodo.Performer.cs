using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using Todo = PagePlay.Site.Application.Todos.Models.Todo;

namespace PagePlay.Site.Application.Todos.Performers.DeleteTodo;

public class DeleteTodoPerformer(
    IValidator<DeleteTodoRequest> _validator,
    ICurrentUserContext currentUserContext,
    IRepository _repository
) : PerformerBase<DeleteTodoRequest, DeleteTodoResponse>, IPerformer<DeleteTodoRequest, DeleteTodoResponse>
{
    public async Task<IApplicationResult<DeleteTodoResponse>> Perform(DeleteTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(currentUserContext.UserId.Value))
            return Fail("You do not have permission to delete this todo.");

        await deleteTodo(todo);

        return Succeed(buildResponse(todo));
    }

    private async Task<ValidationResult> validate(DeleteTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.Get(Todo.ById(id));

    private async Task deleteTodo(Todo todo)
    {
        await _repository.Delete(todo);
        await _repository.SaveChanges();
    }

    private DeleteTodoResponse buildResponse(Todo todo) =>
        new DeleteTodoResponse
        {
            Id = todo.Id,
            Message = "Todo deleted successfully."
        };
}
