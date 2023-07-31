using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IRecommendationProvidable
{
    public Task<ContainerBase?> GetRecommendation(string? typeId = null);
}