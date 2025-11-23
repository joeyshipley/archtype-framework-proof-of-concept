using FluentValidation;
using PagePlay.Site.Application.Todo.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todo.ListTodo;

public class ListTodoWorkflow(
    IValidator<ListTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<ListTodoRequest, ListTodoResponse>
{
    public async Task<IApplicationResult<ListTodoResponse>> Perform(ListTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todos = await getTodosByUserId();

        return response(todos);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(ListTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<List<Domain.Models.Todo>> getTodosByUserId() =>
        await _todoRepository.GetByUserId(_authContext.UserId);

    private IApplicationResult<ListTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<ListTodoResponse>.Fail(validationResult);

    private IApplicationResult<ListTodoResponse> response(List<Domain.Models.Todo> todos) =>
        ApplicationResult<ListTodoResponse>.Succeed(
            new ListTodoResponse
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
