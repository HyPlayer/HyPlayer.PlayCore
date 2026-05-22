using AsyncAwaitBestPractices;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;
using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin : PlayCoreBase
{
    public Chopin(
        IEnumerable<AudioServiceBase> audioServices,
        IEnumerable<ProviderBase> providers,
        IEnumerable<PlayControllerBase> playListControllers,
        IDepository depository)
    {
        SafeFireAndForgetExtensions.Initialize(false);
        _depository = depository;
        AudioServices =
            new(
                new ObservableCollection<AudioServiceBase>(audioServices.ToList()));
        MusicProviders =
            new(
                new ObservableCollection<ProviderBase>(providers.ToList()));
        PlayListControllers =
            new(
                new ObservableCollection<PlayControllerBase>(playListControllers.ToList()));

        CurrentAudioService = AudioServices.FirstOrDefault();
        CurrentPlayListController = PlayListControllers.FirstOrDefault();
        CurrentPlayList = ResolveOptional<PlayListManagerBase>(typeof(PlayListManagerBase));
    }

    public SingleSongBase? LastRequestedPlaybackSong { get; private set; }

    private void ClearCurrentPlayingTicket()
    {
        CurrentPlayingTicket = null;
        LastRequestedPlaybackSong = null;
    }
}
