using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class PlayListControllerBase
{
    public abstract Task AddSongContainerAsync(ContainerBase container);
    public abstract Task RemoveSongContainerAsync(ContainerBase container);
    public abstract Task ClearSongContainersAsync();
    public abstract Task LoadSongContainerAsync(ContainerBase container);
    public abstract Task ClearSongsAsync();
    public abstract Task<SingleSongBase?> MoveNextAsync();
    public abstract Task<SingleSongBase?> MovePreviousAsync();
}