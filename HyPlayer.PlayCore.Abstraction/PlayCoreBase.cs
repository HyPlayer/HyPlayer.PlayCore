using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class PlayCoreBase : IPlayCoreService,
                                     IPlayCorePlaylistModification,
                                     IPlayCorePlayerModification,
                                     IPlayCorePlayPositionModification

{
    public virtual List<AudioServiceBase>? AudioServices { get; protected set; } = null;
    public virtual List<ProviderBase>? MusicProviders { get; protected set; } = null;
    public virtual List<PlayListControllerBase>? PlayListControllers { get; protected set; } = null;
    public virtual AudioServiceBase? CurrentAudioService { get; protected set; } = null;
    public virtual PlayListControllerBase? CurrentPlayListController { get; protected set; } = null;

    public virtual SingleSongBase? CurrentSong { get; protected set; }
    public virtual ContainerBase? CurrentSongContainer { get; protected set; }
    public virtual List<SingleSongBase>? SongList { get; protected set; }
    public virtual AudioTicketBase? CurrentPlayingTicket { get; protected set; }
    public virtual bool IsRandom { get; protected set; }

    public abstract Task RegisterAudioServiceAsync(Type serviceType);
    public abstract Task RegisterMusicProviderAsync(Type serviceType);
    public abstract Task RegisterPlayListControllerAsync(Type serviceType);
    public abstract Task UnregisterAudioServiceAsync(Type serviceType);
    public abstract Task UnregisterMusicProviderAsync(Type serviceType);
    public abstract Task UnregisterPlayListControllerAsync(Type serviceType);
    public abstract Task FocusAudioServiceAsync(Type serviceType);
    public abstract Task FocusPlayListControllerAsync(Type serviceType);
    public abstract Task ChangeSongContainerAsync(ContainerBase? container);
    public abstract Task InsertSongAsync(SingleSongBase item, int index = -1);
    public abstract Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1);
    public abstract Task RemoveSongAsync(SingleSongBase item);
    public abstract Task RemoveSongRangeAsync(List<SingleSongBase> item);
    public abstract Task RemoveAllSongAsync();
    public abstract Task SetRandomAsync(bool isRandom);
    public abstract Task ReRandomAsync();
    public abstract Task SeekAsync(long position);
    public abstract Task PlayAsync();
    public abstract Task PauseAsync();
    public abstract Task StopAsync();
    public abstract Task MovePointerToAsync(SingleSongBase song);
    public abstract Task MoveNextAsync();
    public abstract Task MovePreviousAsync();
}