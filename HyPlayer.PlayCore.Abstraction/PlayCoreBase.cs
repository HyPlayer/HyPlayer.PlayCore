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
    public ReadOnlyCollection<AudioServiceBase>? AudioServices { get; private set; }
    public ReadOnlyCollection<ProviderBase>? MusicProviders { get; private set; }
    public ReadOnlyCollection<PlayListControllerBase>? PlayListControllers { get; private set; }
    public AudioServiceBase? CurrentAudioService { get; private set; }
    public PlayListControllerBase? CurrentPlayListController { get; private set; }

    public SingleSongBase? CurrentSong { get; private set; }
    public SongContainerBase? CurrentSongContainer { get; private set; }
    public ObservableCollection<SingleSongBase>? SongList { get; private set; }
    public AudioTicketBase? CurrentPlayingTicket { get; private set; }
    public abstract Task RegisterAudioService(Type serviceType);
    public abstract Task RegisterMusicProvider(Type serviceType);
    public abstract Task RegisterPlayListController(Type serviceType);
    public abstract Task UnregisterAudioService(Type serviceType);
    public abstract Task UnregisterMusicProvider(Type serviceType);
    public abstract Task UnregisterPlayListController(Type serviceType);
    public abstract Task FocusAudioService(Type serviceType);
    public abstract Task FocusPlayListController(Type serviceType);
    public abstract Task ChangeSongContainer(SongContainerBase? container);
    public abstract Task InsertSong(SingleSongBase item, int index = -1);
    public abstract Task InsertSongRange(ReadOnlyCollection<SingleSongBase> items, int index = -1);
    public abstract Task RemoveSong(SingleSongBase item);
    public abstract Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> item);
    public abstract Task RemoveAllSong();
    public abstract Task Seek(long position);
    public abstract Task Play();
    public abstract Task Pause();
    public abstract Task Stop();
    public abstract Task MovePointerTo(SingleSongBase song);
    public abstract Task MoveNext();
    public abstract Task MovePrevious();
}