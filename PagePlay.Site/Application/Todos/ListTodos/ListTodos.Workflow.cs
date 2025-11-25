using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Application.Todos.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todos.ListTodos;

public class ListTodosWorkflow(
    IValidator<ListTodosRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : WorkflowBase<ListTodosRequest, ListTodosResponse>, IWorkflow<ListTodosRequest, ListTodosResponse>
{
    public async Task<IApplicationResult<ListTodosResponse>> Perform(ListTodosRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        var todos = await getTodosByUserId();

        return Succeed(buildResponse(todos));
    }

    private async Task<ValidationResult> validate(ListTodosRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<List<Todo>> getTodosByUserId() =>
        await _todoRepository.List(TodoSpecifications.ByUserId(_authContext.UserId));

    private ListTodosResponse buildResponse(List<Todo> todos) =>
        new ListTodosResponse
        {
            Todos = todos
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => new TodoItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList()
        };
}
