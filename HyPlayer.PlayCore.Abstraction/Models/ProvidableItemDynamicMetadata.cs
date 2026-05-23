namespace HyPlayer.PlayCore.Abstraction.Models;

/// <summary>
/// Describes provider-neutral metadata that can change independently of an item's core identity.
/// </summary>
public sealed class ProvidableItemDynamicMetadata
{
    /// <summary>
    /// Gets or sets the number of comments associated with the item, when available.
    /// </summary>
    public long? CommentCount { get; set; }

    /// <summary>
    /// Gets or sets the number of shares associated with the item, when available.
    /// </summary>
    public long? ShareCount { get; set; }

    /// <summary>
    /// Gets or sets the number of liked or subscribed users, when available.
    /// </summary>
    public long? LikedCount { get; set; }

    /// <summary>
    /// Gets or sets whether the current provider session has liked or subscribed to the item.
    /// </summary>
    public bool? IsLiked { get; set; }
}
