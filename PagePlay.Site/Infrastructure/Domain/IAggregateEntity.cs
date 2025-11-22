namespace PagePlay.Site.Infrastructure.Domain;

public interface IAggregateEntity
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}