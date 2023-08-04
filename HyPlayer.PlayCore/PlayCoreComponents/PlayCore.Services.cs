using System.Collections;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin :
    INotifyDependencyChanged<IEnumerable<AudioServiceBase>>,
    INotifyDependencyChanged<IEnumerable<ProviderBase>>,
    INotifyDependencyChanged<IEnumerable<PlayListControllerBase>>,
    INotifyDependencyChanged<AudioServiceBase>,
    INotifyDependencyChanged<PlayListControllerBase>
{
    public override List<AudioServiceBase>? AudioServices { get; protected set; }

    public override List<ProviderBase>? MusicProviders { get; protected set; }

    public override List<PlayListControllerBase>? PlayListControllers { get; protected set; }

    public override AudioServiceBase? CurrentAudioService { get; protected set; }

    public override PlayListControllerBase? CurrentPlayListController { get; protected set; }


    private readonly IDepository _depository;

    private static readonly DependencyDescription _audioServiceDescription =
        new(typeof(AudioServiceBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription _providerDescription =
        new(typeof(ProviderBase), DependencyLifetime.Singleton);

    private static readonly DependencyDescription _playListControllerDescription =
        new(typeof(PlayListControllerBase), DependencyLifetime.Singleton);

    public override Task RegisterAudioServiceAsync(Type serviceType)
    {
        if (_depository.GetDependency(typeof(AudioServiceBase)) is null)
            _depository.AddDependency(_audioServiceDescription);
        _depository.AddRelation(_audioServiceDescription, new(serviceType));
        var depDesc = new DependencyDescription(serviceType, DependencyLifetime.Singleton);
        _depository.AddDependency(depDesc);
        _depository.AddRelation(depDesc, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task RegisterMusicProviderAsync(Type serviceType)
    {
        if (_depository.GetDependency(typeof(ProviderBase)) is null)
            _depository.AddDependency(_providerDescription);
        _depository.AddRelation(_providerDescription, new(serviceType));
        var depDesc = new DependencyDescription(serviceType, DependencyLifetime.Singleton);
        _depository.AddDependency(depDesc);
        _depository.AddRelation(depDesc, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task RegisterPlayListControllerAsync(Type serviceType)
    {
        if (_depository.GetDependency(typeof(PlayListControllerBase)) is null)
            _depository.AddDependency(_playListControllerDescription);
        _depository.AddRelation(_playListControllerDescription, new(serviceType));
        var depDesc = new DependencyDescription(serviceType, DependencyLifetime.Singleton);
        _depository.AddDependency(depDesc);
        _depository.AddRelation(depDesc, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task UnregisterAudioServiceAsync(Type serviceType)
    {
        _depository.DeleteRelation(
            _audioServiceDescription,
            new(serviceType));
        var depDesc = _depository.GetDependency(serviceType);
        if (depDesc is null) return Task.CompletedTask;
        _depository.ClearRelations(depDesc);
        _depository.DeleteDependency(depDesc);

        return Task.CompletedTask;
    }

    public override Task UnregisterMusicProviderAsync(Type serviceType)
    {
        _depository.DeleteRelation(
            _providerDescription,
            new(serviceType));
        var depDesc = _depository.GetDependency(serviceType);
        if (depDesc is null) return Task.CompletedTask;
        _depository.ClearRelations(depDesc);
        _depository.DeleteDependency(depDesc);
        return Task.CompletedTask;
    }

    public override Task UnregisterPlayListControllerAsync(Type serviceType)
    {
        _depository.DeleteRelation(
            _providerDescription,
            new(serviceType));
        var depDesc = _depository.GetDependency(serviceType);
        if (depDesc is null) return Task.CompletedTask;
        _depository.ClearRelations(depDesc);
        _depository.DeleteDependency(depDesc);
        return Task.CompletedTask;
    }

    public override Task FocusAudioServiceAsync(Type serviceType)
    {
        _depository.ChangeFocusingRelation(_audioServiceDescription, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task FocusPlayListControllerAsync(Type serviceType)
    {
        _depository.ChangeFocusingRelation(_playListControllerDescription,
                                           new(serviceType));
        return Task.CompletedTask;
    }

    public void OnDependencyChanged(IEnumerable<AudioServiceBase>? marker)
    {
        AudioServices = new(
            (_depository.ResolveDependencies(typeof(IEnumerable<AudioServiceBase>)))
            .Select(o => (AudioServiceBase)o)
            .ToList());
    }

    public void OnDependencyChanged(IEnumerable<ProviderBase>? marker)
    {
        MusicProviders = new(
            (_depository.ResolveDependencies(typeof(IEnumerable<ProviderBase>)))
            .Select(o => (ProviderBase)o)
            .ToList());
    }

    public void OnDependencyChanged(IEnumerable<PlayListControllerBase>? marker)
    {
        PlayListControllers = new(
            (_depository.ResolveDependencies(typeof(IEnumerable<PlayListControllerBase>)))
            .Select(o => (PlayListControllerBase)o)
            .ToList());
    }

    public void OnDependencyChanged(AudioServiceBase? marker)
    {
        CurrentAudioService
            = (AudioServiceBase)_depository.ResolveDependency(typeof(AudioServiceBase));
    }

    public void OnDependencyChanged(PlayListControllerBase? marker)
    {
        CurrentPlayListController
            = (PlayListControllerBase)_depository.ResolveDependency(typeof(PlayListControllerBase));
    }
}