using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using Todo = PagePlay.Site.Application.Todos.Models.Todo;

namespace PagePlay.Site.Application.Todos.Performers.ToggleTodo;

public class ToggleTodoPerformer(
    IValidator<ToggleTodoRequest> _validator,
    ICurrentUserContext currentUserContext,
    IRepository _repository
) : PerformerBase<ToggleTodoRequest, ToggleTodoResponse>, IPerformer<ToggleTodoRequest, ToggleTodoResponse>
{
    public async Task<IApplicationResult<ToggleTodoResponse>> Perform(ToggleTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todo = await getTodoById(request.Id);
        if (todo == null)
            return Fail("Todo not found.");

        if (!todo.IsOwnedBy(currentUserContext.UserId.Value))
            return Fail("You do not have permission to modify this todo.");

        toggleTodo(todo);
        await saveTodo(todo);

        return Succeed(new ToggleTodoResponse());
    }

    private async Task<ValidationResult> validate(ToggleTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<Todo> getTodoById(long id) =>
        await _repository.GetForUpdate(Todo.ById(id));

    private void toggleTodo(Todo todo) =>
        todo.Toggle();

    private async Task saveTodo(Todo todo) =>
        await _repository.SaveChanges();
}
