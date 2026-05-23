using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides paged listing of items inside provider containers.
/// </summary>
public interface IContainerPageProvidable : IProvider
{
    /// <summary>
    /// Gets one page of items from a provider container.
    /// </summary>
    /// <param name="containerId">The provider-scoped container id.</param>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of items to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of providable items.</returns>
    public Task<ProviderPageResult<ProvidableItemBase>> GetContainerItemsPageAsync(string containerId, int offset, int count, CancellationToken ctk = new());
}
