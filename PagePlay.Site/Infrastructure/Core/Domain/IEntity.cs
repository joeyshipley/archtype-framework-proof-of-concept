namespace PagePlay.Site.Infrastructure.Core.Domain;

public interface IEntity
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}