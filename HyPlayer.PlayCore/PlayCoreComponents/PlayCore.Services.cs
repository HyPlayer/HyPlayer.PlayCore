using System.Collections;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
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

    private static readonly DependencyDescription _audioServiceDescription =
        new(typeof(AudioServiceBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription _providerDescription =
        new(typeof(ProviderBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription _playListControllerDescription =
        new(typeof(PlayListControllerBase), DependencyLifetime.Singleton);
    
    public override async Task RegisterAudioService(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(AudioServiceBase)) is null)
            await _depository.AddDependencyAsync(_audioServiceDescription);
        await _depository.AddRelationAsync(_audioServiceDescription, new(serviceType));
    }

    public override async Task RegisterMusicProvider(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(ProviderBase)) is null)
            await _depository.AddDependencyAsync(_providerDescription);
        await _depository.AddRelationAsync(_providerDescription, new(serviceType));
    }

    public override async Task RegisterPlayListController(Type serviceType)
    {
        if (await _depository.GetDependencyAsync(typeof(PlayListControllerBase)) is null)
            await _depository.AddDependencyAsync(_playListControllerDescription);
        await _depository.AddRelationAsync(_playListControllerDescription, new(serviceType));
    }

    public override async Task UnregisterAudioService(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            _audioServiceDescription,
            new(serviceType));
    }

    public override async Task UnregisterMusicProvider(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            _providerDescription,
            new(serviceType));
    }

    public override async Task UnregisterPlayListController(Type serviceType)
    {
        await _depository.DeleteRelationAsync(
            _providerDescription,
            new(serviceType));
    }

    public override async Task FocusAudioService(Type serviceType)
    {
        await _depository.ChangeFocusingRelationAsync(_audioServiceDescription, new(serviceType));
    }

    public override async Task FocusPlayListController(Type serviceType)
    {
        await _depository.ChangeFocusingRelationAsync(_playListControllerDescription,
                                                      new(serviceType));
    }

    public async Task OnDependencyChanged(IEnumerable<AudioServiceBase>? marker)
    {
        AudioServices = new(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<AudioServiceBase>)))
            .Select(o => (AudioServiceBase)o)
            .ToList());
    }

    public async Task OnDependencyChanged(IEnumerable<ProviderBase>? marker)
    {
        MusicProviders = new(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<ProviderBase>)))
            .Select(o => (ProviderBase)o)
            .ToList());
    }

    public async Task OnDependencyChanged(IEnumerable<PlayListControllerBase>? marker)
    {
        PlayListControllers = new(
            (await _depository.ResolveDependenciesAsync(typeof(IEnumerable<PlayListControllerBase>)))
            .Select(o => (PlayListControllerBase)o)
            .ToList());
    }

    public async Task OnDependencyChanged(AudioServiceBase? marker)
    {
        CurrentAudioService
            = (AudioServiceBase)await _depository.ResolveDependencyAsync(typeof(AudioServiceBase));
    }

    public async Task OnDependencyChanged(PlayListControllerBase? marker)
    {
        CurrentPlayListController
            = (PlayListControllerBase)await _depository.ResolveDependencyAsync(typeof(PlayListControllerBase));
    }
}