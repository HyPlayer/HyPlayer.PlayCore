using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral rich media detail, feed, and resource operations.
/// </summary>
public interface IRichMediaProvidable : IProvider
{
    /// <summary>
    /// Gets provider details for a rich media item such as a video, MV, or short-form media entry.
    /// </summary>
    /// <param name="mediaId">The provider-scoped rich media id.</param>
    /// <param name="typeId">The provider-neutral rich media type id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider rich media item.</returns>
    public Task<RichMediaBase?> GetRichMediaAsync(string mediaId, string typeId, CancellationToken ctk = new());

    /// <summary>
    /// Gets the playable resource for a rich media item.
    /// </summary>
    /// <param name="mediaId">The provider-scoped rich media id.</param>
    /// <param name="typeId">The provider-neutral rich media type id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider-neutral media resource.</returns>
    public Task<ResourceBase?> GetRichMediaResourceAsync(string mediaId, string typeId, CancellationToken ctk = new());

    /// <summary>
    /// Gets provider recommendations for rich media feeds.
    /// </summary>
    /// <param name="typeId">The provider-neutral rich media type id, or <see langword="null" /> for the provider default.</param>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of items to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of rich media items.</returns>
    public Task<ProviderPageResult<RichMediaBase>> GetRichMediaFeedAsync(string? typeId, int offset, int count, CancellationToken ctk = new());
}
