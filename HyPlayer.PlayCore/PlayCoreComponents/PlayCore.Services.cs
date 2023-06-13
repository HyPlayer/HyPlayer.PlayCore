using System.Collections;
using System.Collections.ObjectModel;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore;

public partial class Chopin :
    INotifyDependencyChanged<IEnumerable<AudioServiceBase>>,
    INotifyDependencyChanged<IEnumerable<ProviderBase>>,
    INotifyDependencyChanged<IEnumerable<PlayListControllerBase>>,
    INotifyDependencyChanged<AudioServiceBase>,
    INotifyDependencyChanged<PlayListControllerBase>
{
    public new ReadOnlyCollection<AudioServiceBase>? AudioServices
    {
        get => _audioServices;
        set => SetField(ref _audioServices, value);
    }

    public new ReadOnlyCollection<ProviderBase>? MusicProviders
    {
        get => _musicProviders;
        set => SetField(ref _musicProviders, value);
    }

    public new ReadOnlyCollection<PlayListControllerBase>? PlayListControllers
    {
        get => _playListControllers;
        set => SetField(ref _playListControllers, value);
    }

    public new AudioServiceBase? CurrentAudioService
    {
        get => _currentAudioService;
        private set => SetField(ref _currentAudioService, value);
    }

    public new PlayListControllerBase? CurrentPlayListController
    {
        get => _currentPlayListController;
        private set => SetField(ref _currentPlayListController, value);
    }


    private readonly IDepository _depository;
    private ReadOnlyCollection<AudioServiceBase>? _audioServices;
    private ReadOnlyCollection<ProviderBase>? _musicProviders;
    private ReadOnlyCollection<PlayListControllerBase>? _playListControllers;
    private AudioServiceBase? _currentAudioService;
    private PlayListControllerBase? _currentPlayListController;

    private static readonly DependencyDescription AudioServiceDescription =
        new DependencyDescription(typeof(AudioServiceBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription ProviderDescription =
        new DependencyDescription(typeof(ProviderBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription PlayListControllerDescription =
        new DependencyDescription(typeof(PlayListControllerBase), DependencyLifetime.Singleton);

    public Chopin(
        IEnumerable<AudioServiceBase> audioServices,
        IEnumerable<ProviderBase> providers,
        IEnumerable<PlayListControllerBase> playListControllers,
        IDepository depository)
    {
        _depository = depository;
        AudioServices =
            new ReadOnlyCollection<AudioServiceBase>(
                new ObservableCollection<AudioServiceBase>(audioServices.ToList()));
        MusicProviders =
            new ReadOnlyCollection<ProviderBase>(
                new ObservableCollection<ProviderBase>(providers.ToList()));
        PlayListControllers =
            new ReadOnlyCollection<PlayListControllerBase>(
                new ObservableCollection<PlayListControllerBase>(playListControllers.ToList()));
    }

    public override async Task RegisterAudioService(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(AudioServiceBase)) is null)
            await _depository.AddDependencyAsync(AudioServiceDescription);
        await _depository.AddRelationAsync(AudioServiceDescription, new DependencyRelation(serviceType));
    }

    public override async Task RegisterMusicProvider(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(ProviderBase)) is null)
            await _depository.AddDependencyAsync(ProviderDescription);
        await _depository.AddRelationAsync(ProviderDescription, new DependencyRelation(serviceType));
    }

    public override async Task RegisterPlayListController(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(PlayListControllerBase)) is null)
            await _depository.AddDependencyAsync(PlayListControllerDescription);
        await _depository.AddRelationAsync(PlayListControllerDescription, new DependencyRelation(serviceType));
    }

    public override async Task UnregisterAudioService(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            AudioServiceDescription,
            new DependencyRelation(serviceType));
    }

    public override async Task UnregisterMusicProvider(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            ProviderDescription,
            new DependencyRelation(serviceType));
    }

    public override async Task UnregisterPlayListController(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            ProviderDescription,
            new DependencyRelation(serviceType));
    }

    public override async Task FocusAudioService(Type serviceType)
    {
        await _depository.ChangeFocusingRelationAsync(AudioServiceDescription, new DependencyRelation(serviceType));
    }

    public override async Task FocusPlayListController(Type serviceType)
    {
        await _depository.ChangeFocusingRelationAsync(PlayListControllerDescription,
            new DependencyRelation(serviceType));
    }

    public async Task OnDependencyChanged(IEnumerable<AudioServiceBase>? marker)
    {
        AudioServices = new ReadOnlyCollection<AudioServiceBase>(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<AudioServiceBase>)))
            .Select(o => (AudioServiceBase)o).ToList());
    }

    public async Task OnDependencyChanged(IEnumerable<ProviderBase>? marker)
    {
        MusicProviders = new ReadOnlyCollection<ProviderBase>(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<ProviderBase>)))
            .Select(o => (ProviderBase)o).ToList());
    }

    public async Task OnDependencyChanged(IEnumerable<PlayListControllerBase>? marker)
    {
        PlayListControllers = new ReadOnlyCollection<PlayListControllerBase>(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<PlayListControllerBase>)))
            .Select(o => (PlayListControllerBase)o).ToList());
    }

    public async Task OnDependencyChanged(AudioServiceBase? marker)
    {
        CurrentAudioService = (AudioServiceBase)await _depository.ResolveDependencyAsync(typeof(AudioServiceBase));
    }

    public async Task OnDependencyChanged(PlayListControllerBase? marker)
    {
        CurrentPlayListController = (PlayListControllerBase)await _depository.ResolveDependencyAsync(typeof(PlayListControllerBase));
    }
}