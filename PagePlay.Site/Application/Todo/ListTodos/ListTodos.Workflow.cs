using FluentValidation;
using PagePlay.Site.Application.Todo.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todo.ListTodos;

public class ListTodosWorkflow(
    IValidator<ListTodosRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<ListTodosRequest, ListTodosResponse>
{
    public async Task<IApplicationResult<ListTodosResponse>> Perform(ListTodosRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todos = await getTodosByUserId();

        return response(todos);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(ListTodosRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<List<Domain.Models.Todo>> getTodosByUserId() =>
        await _todoRepository.GetByUserId(_authContext.UserId);

    private IApplicationResult<ListTodosResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<ListTodosResponse>.Fail(validationResult);

    private IApplicationResult<ListTodosResponse> response(List<Domain.Models.Todo> todos) =>
        ApplicationResult<ListTodosResponse>.Succeed(
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
            }
        );
}
