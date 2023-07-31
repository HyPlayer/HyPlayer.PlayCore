using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface INavigateSongPlayListController
{
    public abstract Task NavigateSongTo(SingleSongBase song);
}