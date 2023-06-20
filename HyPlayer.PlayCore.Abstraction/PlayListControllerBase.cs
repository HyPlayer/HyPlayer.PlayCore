using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class PlayListControllerBase
{
    public abstract Task AddSongContainer(ContainerBase container);
    public abstract Task RemoveSongContainer(ContainerBase container);
    public abstract Task ClearSongContainers();
    public abstract Task LoadSongContainer(ContainerBase container);
    public abstract Task ClearSongs();
    public abstract Task<SingleSongBase?> MoveNext();
    public abstract Task<SingleSongBase?> MovePrevious();
}