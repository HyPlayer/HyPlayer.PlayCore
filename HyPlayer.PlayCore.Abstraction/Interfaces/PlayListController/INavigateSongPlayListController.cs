using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface INavigateSongPlayListController
{
    public abstract Task NavigateSongTo(SingleSongBase song);
}