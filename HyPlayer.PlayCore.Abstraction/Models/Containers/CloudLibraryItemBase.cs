namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

/// <summary>
/// Represents a provider-neutral uploaded or cloud-library item.
/// </summary>
public abstract class CloudLibraryItemBase : ProvidableItemBase
{
    /// <summary>
    /// Gets or sets the uploaded file size in bytes when the provider exposes one.
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Gets or sets the time the provider accepted the item into the library.
    /// </summary>
    public DateTimeOffset? UploadedAt { get; set; }
}
