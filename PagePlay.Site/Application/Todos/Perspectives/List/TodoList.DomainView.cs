using PagePlay.Site.Application.Todos.Models;

namespace PagePlay.Site.Application.Todos.Perspectives.List;

public class TodosListDomainView
{
    public const string DomainName = "todosList";
    
    public List<TodoListEntry> List { get; set; } = new();

    public int OpenCount { get; set; }

    public int TotalCount { get; set; }

    public double CompletionRate { get; set; }
}
