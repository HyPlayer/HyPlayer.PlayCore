using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListContainer;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Interfaces.Provider;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Containers;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Tests;

internal static class TestAssert
{
    public static void Ensure(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}

internal sealed class TestAudioService(string id = "audio.test") : AudioServiceBase,
                                                                 IPlayAudioTicketService,
                                                                 IPauseAudioTicketService,
                                                                 IStopAudioTicketService,
                                                                 IAudioTicketSeekableService
{
    public override string Id => id;
    public override string Name => "Test Audio";
    public int CreatedTicketCount { get; private set; }
    public AudioTicketBase? PlayedTicket { get; private set; }
    public AudioTicketBase? PausedTicket { get; private set; }
    public AudioTicketBase? StoppedTicket { get; private set; }
    public double? LastSeekPosition { get; private set; }
    public List<AudioTicketBase> DisposedTickets { get; } = [];

    public override Task<AudioTicketBase> GetAudioTicketAsync(MusicResourceBase musicResource, CancellationToken ctk = new())
    {
        CreatedTicketCount++;
        return Task.FromResult<AudioTicketBase>(new TestAudioTicket
        {
            AudioServiceId = Id,
            MusicResource = musicResource,
            Status = AudioTicketStatus.None
        });
    }

    public override Task DisposeAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = new())
    {
        DisposedTickets.Add(audioTicket);
        return Task.CompletedTask;
    }

    public override Task<List<AudioTicketBase>> GetCreatedAudioTicketsAsync(CancellationToken ctk = new())
        => Task.FromResult(new List<AudioTicketBase>());

    public Task PlayAudioTicketAsync(AudioTicketBase ticket, CancellationToken ctk = new())
    {
        PlayedTicket = ticket;
        ticket.Status = AudioTicketStatus.Playing;
        return Task.CompletedTask;
    }

    public Task PauseAudioTicketAsync(AudioTicketBase ticket, CancellationToken ctk = new())
    {
        PausedTicket = ticket;
        return Task.CompletedTask;
    }

    public Task StopTicketAsync(AudioTicketBase ticket, CancellationToken ctk = new())
    {
        StoppedTicket = ticket;
        return Task.CompletedTask;
    }

    public Task SeekAudioTicketAsync(AudioTicketBase audioTicket, double position, CancellationToken ctk = new())
    {
        LastSeekPosition = position;
        return Task.CompletedTask;
    }
}

internal sealed class TestProvider(MusicResourceBase? resource = null) : ProviderBase, IMusicResourceProvidable
{
    public override string Name => "Provider";
    public override string Id => "provider.test";
    public override List<ProvidableTypeId> ProvidableTypeIds => [];

    public Task<MusicResourceBase?> GetMusicResourceAsync(SingleSongBase song, ResourceQualityTag qualityTag, CancellationToken ctk = new())
        => Task.FromResult(resource);
}

internal sealed class TestPlayController(SingleSongBase? nextSong) : PlayControllerBase
{
    public override Task<SingleSongBase?> MoveNextAsync(CancellationToken ctk = new()) => Task.FromResult(nextSong);
    public override Task<SingleSongBase?> MovePreviousAsync(CancellationToken ctk = new()) => Task.FromResult(nextSong);
    public override Task<SingleSongBase?> MoveToIndexAsync(int index, CancellationToken ctk = new()) => Task.FromResult(nextSong);
}

internal sealed class SequencePlayController(List<SingleSongBase> songs) : PlayControllerBase
{
    private int _index = -1;

    public override Task<SingleSongBase?> MoveNextAsync(CancellationToken ctk = new())
    {
        _index++;
        return Task.FromResult<SingleSongBase?>(songs[_index]);
    }

    public override Task<SingleSongBase?> MovePreviousAsync(CancellationToken ctk = new()) => MoveNextAsync(ctk);
    public override Task<SingleSongBase?> MoveToIndexAsync(int index, CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(songs[index]);
}

internal sealed class NavigablePlayController : PlayControllerBase, INavigateSongPlayListController
{
    public SingleSongBase? NavigatedSong { get; private set; }
    public override Task<SingleSongBase?> MoveNextAsync(CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);
    public override Task<SingleSongBase?> MovePreviousAsync(CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);
    public override Task<SingleSongBase?> MoveToIndexAsync(int index, CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);

    public Task NavigateSongToAsync(SingleSongBase song, CancellationToken ctk = new())
    {
        NavigatedSong = song;
        return Task.CompletedTask;
    }
}

internal sealed class RandomizablePlayController : PlayControllerBase, IRandomizablePlayListController
{
    public List<int> Seeds { get; } = [];
    public override Task<SingleSongBase?> MoveNextAsync(CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);
    public override Task<SingleSongBase?> MovePreviousAsync(CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);
    public override Task<SingleSongBase?> MoveToIndexAsync(int index, CancellationToken ctk = new()) => Task.FromResult<SingleSongBase?>(null);

    public Task RandomizeAsync(int seed = -1, CancellationToken ctk = new())
    {
        Seeds.Add(seed);
        return Task.CompletedTask;
    }

    public Task<List<SingleSongBase>> GetOriginalListAsync(CancellationToken ctk = new()) => Task.FromResult(new List<SingleSongBase>());
}

internal sealed class TestPlayListManager(List<SingleSongBase> songs) : PlayListManagerBase
{
    public List<SingleSongBase> AddedSongs { get; } = [];
    public List<List<SingleSongBase>> AddedRanges { get; } = [];
    public List<SingleSongBase> RemovedSongs { get; } = [];
    public List<List<SingleSongBase>> RemovedRanges { get; } = [];
    public int ClearSongsCalled { get; private set; }

    public override Task AddSongContainerAsync(ContainerBase container, CancellationToken ctk = new()) => Task.CompletedTask;
    public override Task RemoveSongContainerAsync(ContainerBase container, CancellationToken ctk = new()) => Task.CompletedTask;
    public override Task<List<ContainerBase>> GetAllSongContainersAsync(CancellationToken ctk = new()) => Task.FromResult(new List<ContainerBase>());
    public override Task ClearSongContainersAsync(CancellationToken ctk = new()) => Task.CompletedTask;
    public override Task<List<SingleSongBase>> GetPlayListAsync(CancellationToken ctk = new()) => Task.FromResult(songs);

    public override Task AddSongAsync(SingleSongBase song, int index = -1, CancellationToken ctk = new())
    {
        AddedSongs.Add(song);
        return Task.CompletedTask;
    }

    public override Task AddSongRangeAsync(List<SingleSongBase> song, int index = -1, CancellationToken ctk = new())
    {
        AddedRanges.Add(song);
        return Task.CompletedTask;
    }

    public override Task RemoveSongAsync(SingleSongBase song, CancellationToken ctk = new())
    {
        RemovedSongs.Add(song);
        return Task.CompletedTask;
    }

    public override Task RemoveSongRangeAsync(List<SingleSongBase> song, CancellationToken ctk = new())
    {
        RemovedRanges.Add(song);
        return Task.CompletedTask;
    }

    public override Task ClearSongsAsync(CancellationToken ctk = new())
    {
        ClearSongsCalled++;
        return Task.CompletedTask;
    }

    public override Task SetSongListAsync(List<SingleSongBase> song, CancellationToken ctk = new()) => Task.CompletedTask;
}

internal sealed class ProgressiveSongContainer(List<ProvidableItemBase> items) : ContainerBase, IProgressiveLoadingContainer
{
    public override string ProviderId => "provider.test";
    public override string TypeId => "container";
    public int MaxProgressiveCount => 1;
    public List<int> RequestedStarts { get; } = [];

    public Task<(bool, List<ProvidableItemBase>)> GetProgressiveItemsListAsync(int start, int count, CancellationToken ctk = new())
    {
        RequestedStarts.Add(start);
        var batch = items.Skip(start).Take(count).ToList();
        return Task.FromResult((start + count < items.Count, batch));
    }
}

internal sealed class TestLinerContainer(List<ProvidableItemBase> items) : LinerContainerBase
{
    public override string ProviderId => "provider.test";
    public override string TypeId => "liner";
    public override Task<List<ProvidableItemBase>> GetAllItemsAsync(CancellationToken ctk = new()) => Task.FromResult(items);
}

internal sealed class TestUndeterminedContainer(List<ProvidableItemBase> items) : UndeterminedContainerBase
{
    public override string ProviderId => "provider.test";
    public override string TypeId => "undetermined";
    public override Task<List<ProvidableItemBase>> GetNextItemsRangeAsync(CancellationToken ctk = new()) => Task.FromResult(items);
}

internal class TestSong : SingleSongBase
{
    public override string ProviderId => "provider.test";
    public override string TypeId => "song";
    public override Task<List<PersonBase>?> GetCreatorsAsync(CancellationToken ctk = new()) => Task.FromResult<List<PersonBase>?>(null);
}

internal sealed class OtherProviderSong : TestSong
{
    public override string ProviderId => "provider.other";
}

internal sealed class TestMusicResource : MusicResourceBase
{
    public override Task<ResourceResultBase> GetResourceAsync(ResourceQualityTag? qualityTag = null, CancellationToken ctk = new())
        => Task.FromResult<ResourceResultBase>(new TestResourceResult { ResourceStatus = ResourceStatus.Success, ExternalException = null });
}

internal sealed class TestResourceResult : ResourceResultBase
{
    public override Exception? ExternalException { get; init; }
    public override required ResourceStatus ResourceStatus { get; init; }
}

internal sealed class TestAudioTicket : AudioTicketBase;

internal sealed class NoopNotificationHub : INotificationHub
{
    public Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = new()) => Task.CompletedTask;
    public Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification, CancellationToken ctk = new()) => Task.FromResult(new List<TResult>());
}

internal sealed class TestDepository(params object[] implementations) : IDepository
{
    private readonly Dictionary<Type, DependencyDescription> _dependencies = [];
    private readonly Dictionary<Type, object?> _targets = implementations.ToDictionary(i => i.GetType(), i => (object?)i);
    private readonly Dictionary<Type, List<DependencyRelation>> _relations = [];
    public List<(Type DependencyType, Type ImplementType)> AddedRelations { get; } = [];
    public List<(Type DependencyType, Type ImplementType)> DeletedRelations { get; } = [];
    public IDepositoryResolveScope RootScope { get; } = new TestResolveScope();

    public void AddDependency(DependencyDescription description) => _dependencies[description.DependencyType] = description;
    public bool DependencyExist(Type dependencyType) => _dependencies.ContainsKey(dependencyType);
    public DependencyDescription? GetDependency(Type dependencyType) => _dependencies.GetValueOrDefault(dependencyType);
    public void DeleteDependency(DependencyDescription description) => _dependencies.Remove(description.DependencyType);
    public void SetDependencyDecoration(DependencyDescription description, DependencyRelation? decorationRelation) => description.DecorationRelation = decorationRelation;
    public void ClearAllDependencies() => _dependencies.Clear();

    public void AddRelation(DependencyDescription dependency, DependencyRelation relation)
    {
        AddedRelations.Add((dependency.DependencyType, relation.ImplementType));
        if (!_relations.TryGetValue(dependency.DependencyType, out var relations))
        {
            relations = [];
            _relations[dependency.DependencyType] = relations;
        }
        relations.Add(relation);
    }

    public DependencyRelation? GetRelation(DependencyDescription description, bool includeDisabled = false, string? relationName = null)
        => GetRelations(description, includeDisabled).FirstOrDefault(r => relationName is null || r.Name == relationName);

    public List<DependencyRelation> GetRelations(DependencyDescription description, bool includeDisabled = false)
        => _relations.GetValueOrDefault(description.DependencyType, [])
            .Where(r => includeDisabled || r.IsEnabled)
            .ToList();

    public void ChangeFocusingRelation(DependencyDescription description, DependencyRelation relation)
    {
        AddRelation(description, relation);
        _targets[description.DependencyType] = ResolveByType(relation.ImplementType);
    }

    public void DeleteRelation(DependencyDescription description, DependencyRelation relation)
    {
        DeletedRelations.Add((description.DependencyType, relation.ImplementType));
        if (_relations.TryGetValue(description.DependencyType, out var relations))
            relations.RemoveAll(r => r.ImplementType == relation.ImplementType && r.Name == relation.Name);
    }

    public void ClearRelations(DependencyDescription description) => _relations.Remove(description.DependencyType);
    public void DisableRelation(DependencyDescription description, DependencyRelation relation) => SetRelationEnabled(description, relation, false);
    public void EnableRelation(DependencyDescription description, DependencyRelation relation) => SetRelationEnabled(description, relation, true);

    public List<object> ResolveDependencies(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType && dependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var itemType = dependency.GetGenericArguments()[0];
            return _targets.Values.Where(v => v is not null && itemType.IsInstanceOfType(v)).Cast<object>().ToList();
        }

        return _targets.Values.Where(v => v is not null && dependency.IsInstanceOfType(v)).Cast<object>().ToList();
    }

    public object ResolveDependency(Type dependency, DependencyResolveOption? option = null)
        => ResolveByType(dependency) ?? throw new InvalidOperationException($"No implementation for {dependency}.");

    public void ChangeResolveTarget(Type dependency, object? target) => _targets[dependency] = target;
    public void RemoveImplementation(Type implementType, string? key = null) => _targets.Remove(implementType);
    public void SetImplementation(Type implementType, object implement, string? key = null) => _targets[implementType] = implement;
    public IDepositoryResolveScope CreateScope(DepositoryResolveScopeOption? option = null) => new TestResolveScope();
    public IDepository CreateDepositoryInScope(IDepositoryResolveScope scope) => this;
    public void Dispose() { }

    private object? ResolveByType(Type type)
        => _targets.TryGetValue(type, out var exact) ? exact : _targets.Values.FirstOrDefault(v => v is not null && type.IsInstanceOfType(v));

    private void SetRelationEnabled(DependencyDescription description, DependencyRelation relation, bool enabled)
    {
        var existing = GetRelations(description, true).FirstOrDefault(r => r.ImplementType == relation.ImplementType && r.Name == relation.Name);
        if (existing is not null)
            existing.IsEnabled = enabled;
    }
}

internal sealed class TestResolveScope : IDepositoryResolveScope
{
    private readonly Dictionary<(Type Type, string? Key), object?> _implementations = [];
    public void SetImplementation(Type type, object? impl, string? key = null) => _implementations[(type, key)] = impl;
    public object? GetImplement(Type type, string? key = null) => _implementations.GetValueOrDefault((type, key));
    public bool Exist(Type type, string? key = null) => _implementations.ContainsKey((type, key));
    public void RemoveImplement(Type type, string? key = null) => _implementations.Remove((type, key));
    public void Dispose() { }
}
