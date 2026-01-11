namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Marker interface for elements that can receive dropped items.
/// Implements the drop zone capability in the Closed-World UI drag-drop system.
/// </summary>
public interface IDropZone : IElement
{
    /// <summary>
    /// The name of this drop zone.
    /// When set, the element renders with data-drop-zone="{name}".
    /// </summary>
    string DropZoneName { get; }
}
