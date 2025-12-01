using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Data;

namespace PagePlay.Site.Application.Todos.Perspectives.List;

public class TodosListProvider(IRepository _repository) : IDataProvider<TodosListDomainView>
{
    public async Task<TodosListDomainView> FetchTyped(long userId)
    {
        var todos = await fetchData(userId);

        return new TodosListDomainView
        {
            List = transformToListEntries(todos),
            OpenCount = calculateOpenCount(todos),
            TotalCount = calculateTotalCount(todos),
            CompletionRate = calculateCompletionRate(todos)
        };
    }

    private async Task<List<Todo>> fetchData(long userId) =>
        await _repository.List(Todo.ByUserId(userId));

    private List<TodoListEntry> transformToListEntries(List<Todo> todos) =>
        todos
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedAt)
            .Select(TodoListEntry.FromTodo)
            .ToList();

    private int calculateOpenCount(List<Todo> todos) =>
        todos.Count(t => !t.IsCompleted);

    private int calculateTotalCount(List<Todo> todos) =>
        todos.Count;

    private double calculateCompletionRate(List<Todo> todos)
    {
        var totalCount = todos.Count;
        return totalCount > 0
            ? (double)todos.Count(t => t.IsCompleted) / totalCount
            : 0.0;
    }

}
