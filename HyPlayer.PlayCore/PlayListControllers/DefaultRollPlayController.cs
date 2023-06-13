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
                                         IPlayListGettablePlaylistContainer
{
    private List<SingleSongBase> _list { get; } = new();
    private SongContainerBase? _currentSongListContainer;
    private int _index = -1;

    private readonly IDepository _depository;

    public DefaultRollPlayController(IDepository depository)
    {
        _depository = depository;
    }

    public Task<ReadOnlyCollection<SingleSongBase>> GetPlayList()
    {
        return Task.FromResult(new ReadOnlyCollection<SingleSongBase>(_list));
    }

    public override async Task AppendSongContainer(SongContainerBase container)
    {
        List<SingleSongBase> songsToBeAdd = new();
        if (container is IProgressiveLoadingContainer progressiveContainer)
        {
            bool isNotEnded = true;
            int startPosition = 0;
            while (isNotEnded)
            {
                var res = await progressiveContainer.GetProgressiveSongList(
                    startPosition, progressiveContainer.MaxProgressiveCount);
                isNotEnded = res.Item1;
                songsToBeAdd.AddRange(res.Item2);
                startPosition += progressiveContainer.MaxProgressiveCount;
            }
        }
        else if (container is LinerSongContainerBase linerContainer)
        {
            songsToBeAdd.AddRange(await linerContainer.GetAllSongs());
            _currentSongListContainer = linerContainer;
        }
        else if (container is UndeterminedSongContainerBase undSongContainer)
        {
            songsToBeAdd.AddRange(await undSongContainer.GetNextSongRange());
        }
        
        _list.AddRange(songsToBeAdd);
        _depository.PublishNotificationAsync(
                       new SongRangeAppendedNotification
                       {
                           Index = _list.Count - songsToBeAdd.Count,
                           AppendedSongs = songsToBeAdd,
                       })
                   .SafeFireAndForget();
    }

    public override Task ClearSongs()
    {
        _list.Clear();
        _depository.PublishNotificationAsync(new PlayListClearedNotification()).SafeFireAndForget();
        return Task.CompletedTask;
    }

    public override Task<SingleSongBase?> MoveNext()
    {
        if (_index == -1) _index = 0;
        if (_index >= _list.Count) _index = 0;
        if (_list.Count == 0)
        {
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = _list[_index]
                           })
                       .SafeFireAndForget();
            return Task.FromResult<SingleSongBase?>(null);
        }

        _index++;
        _depository.PublishNotificationAsync(
                       new CurrentSongChangedNotification
                       {
                           CurrentPlayingSong = _list[_index]
                       })
                   .SafeFireAndForget();
        return Task.FromResult<SingleSongBase?>(_list[_index]);
    }

    public override Task<SingleSongBase?> MovePrevious()
    {
        if (_index == -1) _index = _list.Count - 1;
        if (_index >= _list.Count) _index = _list.Count - 1;
        if (_list.Count == 0)
        {
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = null
                           })
                       .SafeFireAndForget();
            return Task.FromResult<SingleSongBase?>(null);
        }

        if (_index - 1 < 0) _index = _list.Count - 1;
        _index--;
        _depository.PublishNotificationAsync(
                       new CurrentSongChangedNotification
                       {
                           CurrentPlayingSong = _list[_index]
                       })
                   .SafeFireAndForget();
        return Task.FromResult<SingleSongBase?>(_list[_index]);
    }

    public Task Reverse()
    {
        if (_list.Count == 0) return Task.CompletedTask;
        _list.Reverse();
        _index = _list.Count - _index - 1;
        _depository.PublishNotificationAsync(new PlayListChangedNotification()).SafeFireAndForget();
        return Task.CompletedTask;
    }

    public Task InsertSongRange(ReadOnlyCollection<SingleSongBase> song, int index)
    {
        _list.InsertRange(index, song);
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
        var newIndex = _list.IndexOf(song);
        if (newIndex != -1 && newIndex != _index)
        {
            _index = newIndex;
            _depository.PublishNotificationAsync(
                           new CurrentSongChangedNotification
                           {
                               CurrentPlayingSong = _list[_index],
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
        if (_index >= 0 && _index < _list.Count)
        {
            return Task.FromResult<SingleSongBase?>(_list[_index]);
        }

        return Task.FromResult<SingleSongBase?>(null);
    }

    public Task InsertSong(SingleSongBase song, int index)
    {
        _list.Insert(index, song);
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
        _depository.PublishNotificationAsync(
                       new SongRemoveNotification
                       {
                           RemovedSong = song,
                       })
                   .SafeFireAndForget();
        return Task.CompletedTask;
    }
}