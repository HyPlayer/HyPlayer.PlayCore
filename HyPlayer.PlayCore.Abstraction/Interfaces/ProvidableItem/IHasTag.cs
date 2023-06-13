using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasTag
{
    public ReadOnlyCollection<string> Tags { get; set; }
}