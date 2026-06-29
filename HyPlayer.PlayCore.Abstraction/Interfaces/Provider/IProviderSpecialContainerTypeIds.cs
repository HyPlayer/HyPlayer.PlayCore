using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProviderSpecialContainerTypeIds : IProvider
{
    IReadOnlyDictionary<SpecialContainerType, string> SpecialContainerTypeIds { get; }
}
