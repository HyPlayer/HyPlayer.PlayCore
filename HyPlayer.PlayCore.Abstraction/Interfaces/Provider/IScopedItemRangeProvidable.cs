using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides paged item ranges scoped by another provider item, such as artist songs or radio programs.
/// </summary>
public interface IScopedItemRangeProvidable : IProvider
{
    /// <summary>
    /// Gets one page of items associated with a parent provider item.
    /// </summary>
    /// <param name="parentId">The provider-scoped parent item id.</param>
    /// <param name="parentTypeId">The provider-neutral parent item type id.</param>
    /// <param name="itemTypeId">The provider-neutral child item type id to request.</param>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of items to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of child items.</returns>
    public Task<ProviderPageResult<ProvidableItemBase>> GetScopedItemsPageAsync(string parentId, string parentTypeId, string itemTypeId, int offset, int count, CancellationToken ctk = new());
}
