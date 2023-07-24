﻿using System.Collections;
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

    public override Task RegisterAudioService(Type serviceType)
    {
        if (_depository.GetDependency(typeof(AudioServiceBase)) is null)
            _depository.AddDependency(_audioServiceDescription);
        _depository.AddRelation(_audioServiceDescription, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task RegisterMusicProvider(Type serviceType)
    {
        if (_depository.GetDependency(typeof(ProviderBase)) is null)
            _depository.AddDependency(_providerDescription);
        _depository.AddRelation(_providerDescription, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task RegisterPlayListController(Type serviceType)
    {
        if (_depository.GetDependency(typeof(PlayListControllerBase)) is null)
            _depository.AddDependency(_playListControllerDescription);
        _depository.AddRelation(_playListControllerDescription, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task UnregisterAudioService(Type serviceType)
    {
        _depository.DeleteRelation(
            _audioServiceDescription,
            new(serviceType));
        return Task.CompletedTask;
    }

    public override Task UnregisterMusicProvider(Type serviceType)
    {
        _depository.DeleteRelation(
            _providerDescription,
            new(serviceType));
        return Task.CompletedTask;
    }

    public override Task UnregisterPlayListController(Type serviceType)
    {
        _depository.DeleteRelation(
            _providerDescription,
            new(serviceType));
        return Task.CompletedTask;
    }

    public override Task FocusAudioService(Type serviceType)
    {
        _depository.ChangeFocusingRelation(_audioServiceDescription, new(serviceType));
        return Task.CompletedTask;
    }

    public override Task FocusPlayListController(Type serviceType)
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