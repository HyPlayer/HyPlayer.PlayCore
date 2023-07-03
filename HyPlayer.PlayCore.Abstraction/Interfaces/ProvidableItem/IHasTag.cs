using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasTag
{
    public List<string> Tags { get; set; }
}