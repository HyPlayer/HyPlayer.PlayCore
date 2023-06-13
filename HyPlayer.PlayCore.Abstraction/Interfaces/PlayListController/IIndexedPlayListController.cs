using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IIndexedPlayListController
{
    public Task<int> GetCurrentIndex();
    public Task<SingleSongBase?> GetSongAt(int index);
}