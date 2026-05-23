using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides dynamic metadata for providable items.
/// </summary>
public interface IProvidableItemDynamicMetadataProvidable : IProvider
{
    /// <summary>
    /// Gets provider-neutral metadata that may change independently of the item details.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id.</param>
    /// <param name="typeId">The provider-neutral item type id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The dynamic metadata for the item.</returns>
    public Task<ProvidableItemDynamicMetadata> GetDynamicMetadataAsync(string itemId, string typeId, CancellationToken ctk = new());
}
