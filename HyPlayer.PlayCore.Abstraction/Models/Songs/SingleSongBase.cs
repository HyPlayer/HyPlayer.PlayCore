using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Models.Songs;

public abstract class SingleSongBase : ProvidableItemBase, IHasCreators
{
    public AlbumBase? Album { get; set; }
    public ReadOnlyCollection<string>? CreatorList { get; init; }
    public abstract Task<ReadOnlyCollection<PersonBase>?> GetCreators();
    public long Duration { get; set; }
    public bool Available { get; set; }
}