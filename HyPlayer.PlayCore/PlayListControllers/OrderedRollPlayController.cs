using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.PlayListControllers;

public class OrderedRollPlayController : PlayControllerBase,
                                         IReversiblePlayListController,
                                         INavigateSongPlayListController,
                                         IIndexedPlayListController,
                                         IPlayListGettablePlaylistController,
                                         IRandomizablePlayListController,
                                         INotifyDependencyChanged<PlayListManagerBase>,
                                         INotificationSubscriber<InnerPlayListChangedNotification>
{
    private int _index = -1;
    private readonly IDepository _depository;
    private readonly INotificationHub _notificationHub;
    private PlayListManagerBase? _playListManager;
    private List<SingleSongBase> _list = new();
    private List<SingleSongBase> _originalList = new();
    private bool _isRandom;
    private int _randomSeed = -1;

    public OrderedRollPlayController(IDepository depository, PlayListManagerBase? playListManager, INotificationHub notificationHub)
    {
        _depository = depository;
        _playListManager = playListManager;
        _notificationHub = notificationHub;
    }

    public override async Task<SingleSongBase?> MoveNextAsync(CancellationToken ctk = new())
    {
        if (_list.Count == 0) return null;
        if (_list.Count <= _index + 1)
            _index = -1;

        _index++;
        var curSong = _list[_index];
        await PublishCurrentSongChangedAsync(curSong, ctk).ConfigureAwait(false);
        return curSong;
    }

    public override async Task<SingleSongBase?> MovePreviousAsync(CancellationToken ctk = new())
    {
        if (_list.Count == 0) return null;
        if (_index <= 0)
            _index = _list.Count;

        _index--;
        var curSong = _list[_index];
        await PublishCurrentSongChangedAsync(curSong, ctk).ConfigureAwait(false);
        return curSong;
    }

    public override async Task<SingleSongBase?> MoveToIndexAsync(int index, CancellationToken ctk = new())
    {
        if (index < 0 || index >= _list.Count) return null;

        _index = index;
        var curSong = _list[index];
        await PublishCurrentSongChangedAsync(curSong, ctk).ConfigureAwait(false);
        return curSong;
    }

    public async Task Reverse(CancellationToken ctk = new())
    {
        if (_list.Count == 0) return;

        var currentSong = GetCurrentSong();
        _list.Reverse();
        _isRandom = false;
        _randomSeed = -1;
        _index = currentSong is null ? -1 : _list.IndexOf(currentSong);
        await PublishOrderedPlaylistChangedAsync(ctk).ConfigureAwait(false);
    }

    public async Task<SingleSongBase?> NavigateSongToAsync(SingleSongBase song, CancellationToken ctk = new())
    {
        if (_list.Count == 0) return null;

        var indexOfSong = IndexOfSong(_list, song);
        if (indexOfSong < 0)
            return null;

        _index = indexOfSong;
        await PublishCurrentSongChangedAsync(_list[_index], ctk).ConfigureAwait(false);
        return _list[_index];
    }

    public Task<int> GetCurrentIndexAsync(CancellationToken ctk = new())
    {
        return Task.FromResult(_index);
    }

    public Task<SingleSongBase?> GetSongAtAsync(int index, CancellationToken ctk = new())
    {
        if (index < 0 || index >= _list.Count) return Task.FromResult<SingleSongBase?>(null);
        return Task.FromResult<SingleSongBase?>(_list[index]);
    }

    public Task<List<SingleSongBase>> GetOrderedPlayListAsync(CancellationToken ctk = new())
    {
        return Task.FromResult(_list.ToList());
    }

    public async Task RandomizeAsync(int seed = -1, CancellationToken ctk = new())
    {
        var currentSong = GetCurrentSong();
        _isRandom = seed != -1;
        _randomSeed = seed;
        RebuildOrderedList(currentSong);
        await PublishOrderedPlaylistChangedAsync(ctk).ConfigureAwait(false);
    }

    public Task<List<SingleSongBase>> GetOriginalListAsync(CancellationToken ctk = new())
    {
        return Task.FromResult(_originalList.ToList());
    }

    public async void OnDependencyChanged(PlayListManagerBase? alwaysNullMarker)
    {
        var currentSong = GetCurrentSong();
        _playListManager = _depository.ResolveDependency(typeof(PlayListManagerBase)) as PlayListManagerBase;
        _originalList = await (_playListManager?.GetPlayListAsync() ?? Task.FromResult(new List<SingleSongBase>()));
        RebuildOrderedList(currentSong);
        await PublishOrderedPlaylistChangedAsync().ConfigureAwait(false);
    }

    public async Task HandleNotificationAsync(
        InnerPlayListChangedNotification notification,
        CancellationToken ctk = new())
    {
        var currentSong = GetCurrentSong();
        _originalList = await (_playListManager?.GetPlayListAsync(ctk) ?? Task.FromResult(new List<SingleSongBase>()));
        RebuildOrderedList(currentSong);
        await PublishOrderedPlaylistChangedAsync(ctk).ConfigureAwait(false);
    }

    private SingleSongBase? GetCurrentSong()
    {
        return _index >= 0 && _index < _list.Count ? _list[_index] : null;
    }

    private void RebuildOrderedList(SingleSongBase? currentSong)
    {
        _list = _isRandom
            ? ShuffleOriginalList(_randomSeed)
            : _originalList.ToList();

        if (_list.Count == 0)
        {
            _index = -1;
            return;
        }

        var currentIndex = currentSong is null ? -1 : IndexOfSong(_list, currentSong);
        _index = currentIndex >= 0 ? currentIndex : Math.Min(Math.Max(_index, -1), _list.Count - 1);
    }

    private static int IndexOfSong(IReadOnlyList<SingleSongBase> list, SingleSongBase song)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (ReferenceEquals(item, song) ||
                item.ProviderId == song.ProviderId &&
                item.TypeId == song.TypeId &&
                item.ActualId == song.ActualId)
                return i;
        }

        return -1;
    }

    private List<SingleSongBase> ShuffleOriginalList(int seed)
    {
        var random = new Random(seed);
        return _originalList.OrderBy(_ => random.Next()).ToList();
    }

    private Task PublishCurrentSongChangedAsync(SingleSongBase currentSong, CancellationToken ctk)
    {
        return _notificationHub.PublishNotificationAsync(
            new CurrentSongChangedNotification { CurrentPlayingSong = currentSong },
            ctk);
    }

    private Task PublishOrderedPlaylistChangedAsync(CancellationToken ctk = new())
    {
        return _notificationHub.PublishNotificationAsync(
            new OrderedPlaylistChangedNotification { IsRandom = _isRandom, OrderedList = _list.ToList() },
            ctk);
    }
}
