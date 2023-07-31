using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IRangeControllablePlayListController
{
    public abstract Task InsertSongRange(List<SingleSongBase> song, int index);
    public abstract Task RemoveSongRange(List<SingleSongBase> song);
}