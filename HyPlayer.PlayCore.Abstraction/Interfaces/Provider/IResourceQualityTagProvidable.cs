using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IResourceQualityTagProvidable : IProvider
{
    Task<IReadOnlyDictionary<string, ResourceQualityTag>> GetAvailableQualityTagsAsync(ResourceType type, CancellationToken ctk = default);
}
