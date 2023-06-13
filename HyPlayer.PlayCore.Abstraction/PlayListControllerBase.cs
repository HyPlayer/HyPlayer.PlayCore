using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class PlayListControllerBase
{
    public abstract Task AppendSongContainer(SongContainerBase songs);
    public abstract Task ClearSongs();
    public abstract Task<SingleSongBase?> MoveNext();
    public abstract Task<SingleSongBase?> MovePrevious();
}