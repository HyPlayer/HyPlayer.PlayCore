using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IRangeControllablePlayListController
{
    public abstract Task InsertSongRange(ReadOnlyCollection<SingleSongBase> song, int index);
    public abstract Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> song);
}