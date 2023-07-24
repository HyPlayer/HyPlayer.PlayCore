using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

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

    public abstract Task RegisterAudioService(Type serviceType);
    public abstract Task RegisterMusicProvider(Type serviceType);
    public abstract Task RegisterPlayListController(Type serviceType);
    public abstract Task UnregisterAudioService(Type serviceType);
    public abstract Task UnregisterMusicProvider(Type serviceType);
    public abstract Task UnregisterPlayListController(Type serviceType);
    public abstract Task FocusAudioService(Type serviceType);
    public abstract Task FocusPlayListController(Type serviceType);
    public abstract Task ChangeSongContainer(ContainerBase? container);
    public abstract Task InsertSong(SingleSongBase item, int index = -1);
    public abstract Task InsertSongRange(List<SingleSongBase> items, int index = -1);
    public abstract Task RemoveSong(SingleSongBase item);
    public abstract Task RemoveSongRange(List<SingleSongBase> item);
    public abstract Task RemoveAllSong();
    public abstract Task SetRandom(bool isRandom);
    public abstract Task ReRandom();
    public abstract Task Seek(long position);
    public abstract Task Play();
    public abstract Task Pause();
    public abstract Task Stop();
    public abstract Task MovePointerTo(SingleSongBase song);
    public abstract Task MoveNext();
    public abstract Task MovePrevious();
}