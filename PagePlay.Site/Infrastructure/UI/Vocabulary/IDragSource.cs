namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Marker interface for elements that can be dragged.
/// Implements the drag source capability in the Closed-World UI drag-drop system.
/// </summary>
public interface IDragSource : IElement
{
    /// <summary>
    /// The unique identifier for this drag source.
    /// When set, the element renders with draggable="true" and data-drag-id="{id}".
    /// </summary>
    long? DragSourceId { get; }
}
