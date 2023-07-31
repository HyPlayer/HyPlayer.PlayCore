using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IStoredItemsProvidable
{
    public Task<ContainerBase?> GetStoredItems(string typeId);
}