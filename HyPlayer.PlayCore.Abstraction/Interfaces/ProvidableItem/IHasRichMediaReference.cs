using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasRichMediaReference : IProvidableItem
{
    string? RichMediaId { get; }
}
