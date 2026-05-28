using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IRecommendationProvidable : IProvider
{
    Task<ContainerBase> GetRecommendationAsync(string? typeId = null, CancellationToken ctk = default(CancellationToken));
}
