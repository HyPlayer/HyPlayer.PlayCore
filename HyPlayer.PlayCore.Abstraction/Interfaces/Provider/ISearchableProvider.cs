using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface ISearchableProvider
{
    public Task<ContainerBase> SearchProvidableItems(string keyword,string typeId);
}