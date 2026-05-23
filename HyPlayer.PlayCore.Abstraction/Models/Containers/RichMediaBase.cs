namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

/// <summary>
/// Represents a provider-neutral rich media item such as a video, MV, or short-form media entry.
/// </summary>
public abstract class RichMediaBase : ProvidableItemBase
{
    /// <summary>
    /// Gets or sets the media duration in milliseconds when the provider exposes one.
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// Gets or sets the media description when the provider exposes one.
    /// </summary>
    public string? Description { get; set; }
}
