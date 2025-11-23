using FluentValidation;
using PagePlay.Site.Application.Todo.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Todo.CreateTodo;

public class CreateTodoWorkflow(
    IValidator<CreateTodoRequest> _validator,
    LoggedInAuthContext _authContext,
    ITodoRepository _todoRepository
) : IWorkflow<CreateTodoRequest, CreateTodoResponse>
{
    public async Task<IApplicationResult<CreateTodoResponse>> Perform(CreateTodoRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return response(validationResult);

        var todo = createTodo(request);
        await saveTodo(todo);

        return response(todo);
    }

    private async Task<FluentValidation.Results.ValidationResult> validate(CreateTodoRequest request) =>
        await _validator.ValidateAsync(request);

    private Domain.Models.Todo createTodo(CreateTodoRequest request) =>
        Domain.Models.Todo.Create(_authContext.UserId, request.Title);

    private async Task saveTodo(Domain.Models.Todo todo)
    {
        await _todoRepository.Add(todo);
        await _todoRepository.SaveChanges();
    }

    private IApplicationResult<CreateTodoResponse> response(FluentValidation.Results.ValidationResult validationResult) =>
        ApplicationResult<CreateTodoResponse>.Fail(validationResult);

    private IApplicationResult<CreateTodoResponse> response(Domain.Models.Todo todo) =>
        ApplicationResult<CreateTodoResponse>.Succeed(
            new CreateTodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            }
        );
}
