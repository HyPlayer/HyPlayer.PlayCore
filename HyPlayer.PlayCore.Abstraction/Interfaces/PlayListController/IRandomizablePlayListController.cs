using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IRandomizablePlayListController
{
    public Task Randomize(int seed = -1);
    public Task<ReadOnlyCollection<SingleSongBase>> GetOriginalList();
}