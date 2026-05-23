namespace HyPlayer.PlayCore.Abstraction.Models;

/// <summary>
/// Describes a provider-neutral paged result.
/// </summary>
/// <typeparam name="TItem">The item type contained in the page.</typeparam>
public sealed class ProviderPageResult<TItem>
{
    /// <summary>
    /// Gets or sets the returned items.
    /// </summary>
    public required List<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets whether more items are available after this page.
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Gets or sets the next offset to request when <see cref="HasMore" /> is true.
    /// </summary>
    public int? NextOffset { get; set; }

    /// <summary>
    /// Gets or sets the total item count when the provider exposes one.
    /// </summary>
    public int? TotalCount { get; set; }
}
