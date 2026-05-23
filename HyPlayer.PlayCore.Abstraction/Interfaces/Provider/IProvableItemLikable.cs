namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-owned like, favorite, subscribe, and add-to-container operations for providable items.
/// </summary>
public interface IProvableItemLikable : IProvider
{
    /// <summary>
    /// Likes or favorites an item when <paramref name="targetId" /> is <see langword="null" />; otherwise adds the item to the target container.
    /// </summary>
    /// <param name="inProviderId">The provider-scoped item id.</param>
    /// <param name="targetId">The provider-scoped target container id, or <see langword="null" /> for heart/favorite semantics.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task LikeProvidableItemAsync(string inProviderId, string? targetId, CancellationToken ctk = new());

    /// <summary>
    /// Unlikes or unfavorites an item when <paramref name="targetId" /> is <see langword="null" />; otherwise removes the item from the target container.
    /// </summary>
    /// <param name="inProviderId">The provider-scoped item id.</param>
    /// <param name="targetId">The provider-scoped target container id, or <see langword="null" /> for heart/favorite semantics.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task UnlikeProvidableItemAsync(string inProviderId, string? targetId, CancellationToken ctk = new());

    /// <summary>
    /// Gets provider-scoped ids that the current session has liked, followed, or subscribed to for the requested type.
    /// </summary>
    /// <param name="typeId">The provider-neutral item type id to list.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider-scoped liked item ids.</returns>
    public Task<List<string>> GetLikedProvidableIdsAsync(string typeId, CancellationToken ctk = new());
}
