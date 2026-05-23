using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral recommendation variants that depend on an existing item.
/// </summary>
public interface IContextRecommendationProvidable : IProvider
{
    /// <summary>
    /// Gets recommendations related to a provider item.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id used as recommendation context.</param>
    /// <param name="typeId">The provider-neutral recommendation type id.</param>
    /// <param name="count">The maximum number of recommendations to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral recommendation container.</returns>
    public Task<ContainerBase> GetContextRecommendationAsync(string itemId, string typeId, int count, CancellationToken ctk = new());
}
