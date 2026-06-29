using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasAliases : IProvidableItem
{
    IReadOnlyList<string>? Aliases { get; }
}
