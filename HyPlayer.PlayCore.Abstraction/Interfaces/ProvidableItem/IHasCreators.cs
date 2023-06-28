using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasCreators
{
    public ReadOnlyCollection<string>? CreatorList { get; }
    public Task<ReadOnlyCollection<PersonBase>?> GetCreators();
}