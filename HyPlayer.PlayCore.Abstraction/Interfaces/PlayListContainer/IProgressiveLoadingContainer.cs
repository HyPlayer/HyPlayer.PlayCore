using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListContainer;

public interface IProgressiveLoadingContainer
{
    public int MaxProgressiveCount { get; }
    public Task<(bool, ReadOnlyCollection<SingleSongBase>)> GetProgressiveSongList(int start, int count);
}