using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IPlayListGettablePlaylistContainer
{
    public abstract Task<List<SingleSongBase>> GetPlayList();
}