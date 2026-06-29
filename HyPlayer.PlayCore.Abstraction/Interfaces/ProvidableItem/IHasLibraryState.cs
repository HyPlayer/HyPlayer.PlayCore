using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasLibraryState : IProvidableItem
{
    bool IsOwnedByCurrentUser { get; }
    bool IsInCurrentUserLibrary { get; }
}
