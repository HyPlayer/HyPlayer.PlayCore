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
    public virtual List<PlayControllerBase>? PlayListControllers { get; protected set; } = null;
    public virtual AudioServiceBase? CurrentAudioService { get; protected set; } = null;
    public virtual PlayControllerBase? CurrentPlayListController { get; protected set; } = null;

    public virtual SingleSongBase? CurrentSong { get; protected set; }
    public virtual ContainerBase? CurrentSongContainer { get; protected set; }
    public virtual PlayListManagerBase? CurrentPlayList { get; protected set; }
    public virtual AudioTicketBase? CurrentPlayingTicket { get; protected set; }
    public virtual bool IsRandom { get; protected set; }
    public virtual string ActivePlayModeId { get; protected set; } = "seq";
    public virtual string PlaySourceId { get; set; } = string.Empty;

    public abstract Task RegisterAudioServiceAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task RegisterMusicProviderAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task RegisterPlayListControllerAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task UnregisterAudioServiceAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task UnregisterMusicProviderAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task UnregisterPlayListControllerAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task FocusAudioServiceAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task FocusPlayListControllerAsync(Type serviceType, CancellationToken ctk = new());
    public abstract Task ChangeSongContainerAsync(ContainerBase? container, CancellationToken ctk = new());
    public abstract Task InsertSongAsync(SingleSongBase item, int index = -1, CancellationToken ctk = new());
    public abstract Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1, CancellationToken ctk = new());
    public abstract Task RemoveSongAsync(SingleSongBase item, CancellationToken ctk = new());
    public abstract Task RemoveSongRangeAsync(List<SingleSongBase> item, CancellationToken ctk = new());
    public abstract Task RemoveAllSongAsync(CancellationToken ctk = new());
    public abstract Task SetRandomAsync(bool isRandom, CancellationToken ctk = new());
    public abstract Task ReRandomAsync(CancellationToken ctk = new());
    public abstract Task SetPlayModeAsync(string playModeId, CancellationToken ctk = new());
    public abstract Task<List<SingleSongBase>> GetPlaylistAsync(CancellationToken ctk = new());
    public abstract Task<List<SingleSongBase>> GetOrderedPlaylistAsync(CancellationToken ctk = new());
    public abstract Task<int> GetCurrentIndexAsync(CancellationToken ctk = new());
    public abstract Task<SingleSongBase?> GetSongAtAsync(int index, CancellationToken ctk = new());
    public abstract Task ReversePlaylistAsync(CancellationToken ctk = new());
    public abstract Task SeekAsync(long position, CancellationToken ctk = new());
    public abstract Task PlayAsync(CancellationToken ctk = new());
    public abstract Task PauseAsync(CancellationToken ctk = new());
    public abstract Task StopAsync(CancellationToken ctk = new());
    public abstract Task<PreparedPlaybackTicket?> PreparePlaybackAsync(
        SingleSongBase song,
        CancellationToken ctk = new());
    public abstract Task<PreparedPlaybackPromotion?> PromotePreparedPlaybackAsync(
        PreparedPlaybackTicket preparedTicket,
        CancellationToken ctk = new());
    public abstract Task MovePointerToAsync(SingleSongBase song, CancellationToken ctk = new());
    public abstract Task MovePointerToIndexAsync(int index, CancellationToken ctk = new());
    public abstract Task MoveNextAsync(CancellationToken ctk = new());
    public abstract Task MovePreviousAsync(CancellationToken ctk = new());
}

public interface IPlayCore { }
