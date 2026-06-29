namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IPersonalRadioProvidable : IProvider
{
    Task MovePersonalRadioItemToTrashAsync(string itemId, CancellationToken ctk = default);
}
