using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListContainer;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.PlayListControllers;

public class DefaultRollPlayController : PlayListControllerBase,
                                         IReversiblePlayListController,
                                         IRangeControllablePlayListController,
                                         INavigateSongPlayListController,
                                         IIndexedPlayListController,
                                         IInsertablePlaylistController,
                                         IPlayListGettablePlaylistContainer,
                                         IRandomizablePlayListController
{
    private readonly List<SingleSongBase> _list = new();
    private readonly List<ContainerBase> _currentSongListContainers = new();
    private readonly List<SingleSongBase> _randomedList = new();

    private int _index = -1;
    private bool _isRandomList = false;

    private readonly IDepository _depository;

    public DefaultRollPlayController(IDepository depository)
    {
        _depository = depository;
    }

    public Task<ReadOnlyCollection<SingleSongBase>> GetPlayList()
    {
        return Task.FromResult(new ReadOnlyCollection<SingleSongBase>(_list));
    }

    public override Task AddSongContainer(ContainerBase container)
    {
        _currentSongListContainers.RemoveAll(t => t is UndeterminedContainerBase);
        if (container is UndeterminedContainerBase)
        {
            _currentSongListContainers.Clear();
        }

        _currentSongListContainers.Add(container);
        return Task.CompletedTask;
    }

    public override Task RemoveSongContainer(ContainerBase container)
    {
        _currentSongListContainers.Remove(container);
        return Task.CompletedTask;
    }

    public override Task ClearSongContainers()
    {
        _currentSongListContainers.Clear();
        return Task.CompletedTask;
    }

    public override async Task LoadSongContainer(ContainerBase container)
    {
        List<SingleSongBase> songsToBeAdd = new();
        if (container is IProgressiveLoadingContainer progressiveContainer)
        {
            bool isNotEnded = true;
            int startPosition = 0;
            while (isNotEnded)
            {
                var res = await progressiveContainer.GetProgressiveItemsList(
                    startPosition, progressiveContainer.MaxProgressiveCount);
                isNotEnded = res.Item1;
                songsToBeAdd.AddRange(res.Item2.Cast<SingleSongBase>().ToList());
                startPosition += progressiveContainer.MaxProgressiveCount;
            }
        }
        else if (container is LinerContainerBase linerContainer)
        {
            songsToBeAdd.AddRange((await linerContainer.GetAllItems()).Cast<SingleSongBase>());
        }
        else if (container is UndeterminedContainerBase undSongContainer)
        {
            _randomedList.Clear();
            _list.Clear();
            songsToBeAdd.AddRange((await undSongContainer.GetNextItemsRange()).Cast<SingleSongBase>());
            if (_isRandomList)
            {
                await Randomize(-1);
            }
        }

        _list.AddRange(songsToBeAdd);
        if (_isRandomList)
        {
            await Randomize(DateTime.Now.Millisecond);
        }
        else
        {
            _depository.PublishNotificationAsync(
                           new SongRangeAppendedNotification
                           {
                               Index = _list.Count - songsToBeAdd.Count,
                               AppendedSongs = songsToBeAdd,
                           })
                       .SafeFireAndForget();
        }
    }


    public override Task ClearSongs()
    {
        _list.Clear();
        _randomedList.Clear();
        _depository.PublishNotificationAsync(new PlayListClearedNotification()).SafeFireAndForget();
        return Task.CompletedTask;
    }

    public override async Task<SingleSongBase?> MoveNext()
    {
        if (_currentSongListContainers.FirstOrDefault() is UndeterminedContainerBase undc)
        {
            if (_index >= _list.Count - 2)
                _list.AddRange((await undc.GetNextItemsRange()).Cast<SingleSongBase>());
        }

        var targetList = _isRandomList ? _randomedList : _list;
        if (_index == -1) _index = 0;
        if (_index >= targetList.Count) _index = 0;
        if (targetList.Count == 0)
        {
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = targetList[_index],
                           })
                       .SafeFireAndForget();
            return null;
        }

        _index++;
        _depository.PublishNotificationAsync(
                       new CurrentSongChangedNotification
                       {
                           CurrentPlayingSong = targetList[_index]
                       })
                   .SafeFireAndForget();
        return targetList[_index];
    }

    public override Task<SingleSongBase?> MovePrevious()
    {
        var targetList = _isRandomList ? _randomedList : _list;
        if (_index == -1) _index = targetList.Count - 1;
        if (_index >= targetList.Count) _index = targetList.Count - 1;
        if (targetList.Count == 0)
        {
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = null
                           })
                       .SafeFireAndForget();
            return Task.FromResult<SingleSongBase?>(null);
        }

        if (_index - 1 < 0) _index = targetList.Count - 1;
        _index--;
        _depository.PublishNotificationAsync(
                       new CurrentSongChangedNotification
                       {
                           CurrentPlayingSong = targetList[_index]
                       })
                   .SafeFireAndForget();
        return Task.FromResult<SingleSongBase?>(targetList[_index]);
    }

    public Task Reverse()
    {
        if (_isRandomList) return Randomize(DateTime.Now.Millisecond);
        if (_list.Count == 0) return Task.CompletedTask;
        _list.Reverse();
        _index = _list.Count - _index - 1;
        _depository.PublishNotificationAsync(
                       new PlayListChangedNotification()
                       {
                           NewList = new(_list),
                           IsRandom = _isRandomList,
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task InsertSongRange(ReadOnlyCollection<SingleSongBase> song, int index)
    {
        _list.InsertRange(index, song);
        if (_isRandomList) return Randomize(DateTime.Now.Millisecond);
        _depository.PublishNotificationAsync(
                       new SongRangeAppendedNotification
                       {
                           Index = index,
                           AppendedSongs = song.ToList()
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> song)
    {
        foreach (var singleSongBase in song)
        {
            _list.Remove(singleSongBase);
        }

        if (_isRandomList) return Randomize(DateTime.Now.Millisecond);
        _depository.PublishNotificationAsync(
                       new SongRangeRemovedNotification
                       {
                           RemovedSongs = song.ToList(),
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task NavigateSongTo(SingleSongBase song)
    {
        var targetList = _isRandomList ? _randomedList : _list;
        var newIndex = targetList.IndexOf(song);
        if (newIndex != -1 && newIndex != _index)
        {
            _index = newIndex;
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = targetList[_index],
                           })
                       .SafeFireAndForget();
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task<int> GetCurrentIndex()
    {
        return Task.FromResult(_index);
    }

    public Task<SingleSongBase?> GetSongAt(int index)
    {
        var targetList = _isRandomList ? _randomedList : _list;
        if (_index >= 0 && _index < targetList.Count)
        {
            return Task.FromResult<SingleSongBase?>(targetList[_index]);
        }

        return Task.FromResult<SingleSongBase?>(null);
    }

    public Task InsertSong(SingleSongBase song, int index)
    {
        _list.Insert(index, song);
        if (_isRandomList) return Randomize(DateTime.Now.Millisecond);
        _depository.PublishNotificationAsync(
                       new SongAppendNotification
                       {
                           AppendedSong = song,
                           Index = index,
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task RemoveSong(SingleSongBase song)
    {
        _list.Remove(song);
        if (_isRandomList) return Randomize(DateTime.Now.Millisecond);
        _depository.PublishNotificationAsync(
                       new SongRemoveNotification
                       {
                           RemovedSong = song,
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task Randomize(int randomId)
    {
        _randomedList.Clear();
        if (randomId == -1)
        {
            if (_isRandomList == false) return Task.CompletedTask;
            var targetSong = _randomedList[_index];
            _index = _list.IndexOf(targetSong);
            _isRandomList = false;
            _depository.PublishNotificationAsync(
                new PlayListChangedNotification
                {
                    NewList = new(_list),
                    IsRandom = _isRandomList,
                });
        }

        if (_currentSongListContainers.FirstOrDefault() is UndeterminedContainerBase) return Task.CompletedTask;
        _isRandomList = true;
        var oldTargetSong = _list[_index];
        var random = new Random(randomId);
        _randomedList.AddRange(_list.OrderBy(x => random.Next()).ToList());
        _index = _randomedList.IndexOf(oldTargetSong);
        _depository.PublishNotificationAsync(
            new PlayListChangedNotification
            {
                NewList = new(_randomedList),
                IsRandom = _isRandomList,
            });
        return Task.CompletedTask;
    }

    public Task<ReadOnlyCollection<SingleSongBase>> GetOriginalList()
    {
        return Task.FromResult(new ReadOnlyCollection<SingleSongBase>(_list));
    }
}