using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

public abstract class UndeterminedSongContainerBase : SongContainerBase
{
    public abstract Task<List<SingleSongBase>> GetNextSongRange();
}