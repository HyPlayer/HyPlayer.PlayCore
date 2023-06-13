using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

public abstract class LinerSongContainerBase : SongContainerBase
{
    public abstract Task<ReadOnlyCollection<SingleSongBase>> GetAllSongs();
}