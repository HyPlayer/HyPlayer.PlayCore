namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IContainerItemManagementProvidable : IProvider
{
    Task RemoveItemFromContainerAsync(string containerId, string itemId, CancellationToken ctk = new());
}
