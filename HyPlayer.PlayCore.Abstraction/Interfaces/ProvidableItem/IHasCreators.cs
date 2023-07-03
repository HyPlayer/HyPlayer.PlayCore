using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasCreators
{
    public List<string>? CreatorList { get; init; }
    public Task<List<PersonBase>?> GetCreators();
}