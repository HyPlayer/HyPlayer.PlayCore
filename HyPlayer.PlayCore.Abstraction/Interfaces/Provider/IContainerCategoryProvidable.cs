using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral category catalogs for container discovery.
/// </summary>
public interface IContainerCategoryProvidable : IProvider
{
    /// <summary>
    /// Gets categories that can be used to query provider containers.
    /// </summary>
    /// <param name="typeId">The provider-neutral container type id to categorize, or <see langword="null" /> for the provider default.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider categories.</returns>
    public Task<List<ProviderCategory>> GetContainerCategoriesAsync(string? typeId = null, CancellationToken ctk = new());
}
