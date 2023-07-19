using System.Collections.ObjectModel;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin : 
    INotificationSubscriber<CurrentSongChangedNotification>,
    INotificationSubscriber<PlayListChangedNotification>,
    INotificationSubscriber<PlayListClearedNotification>,
    INotificationSubscriber<SongAppendNotification>,
    INotificationSubscriber<SongRangeAppendedNotification>,
    INotificationSubscriber<SongRemoveNotification>,
    INotificationSubscriber<SongRangeRemovedNotification>

{
    public Task HandleNotification(CurrentSongChangedNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        CurrentSong = notification.CurrentPlayingSong;
        return Task.CompletedTask;
    }

    public Task HandleNotification(PlayListChangedNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        IsRandom = notification.IsRandom;
        SongList = new(notification.NewList);
        return Task.CompletedTask;
    }

    public Task HandleNotification(PlayListClearedNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        SongList?.Clear();
        return Task.CompletedTask;
    }

    public Task HandleNotification(SongAppendNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        SongList?.Insert(notification.Index,notification.AppendedSong);
        return Task.CompletedTask;
    }

    public Task HandleNotification(SongRangeAppendedNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        var index = notification.Index;
        if (SongList is null) return Task.CompletedTask;
        foreach (var song in notification.AppendedSongs)
        {
            SongList?.Insert(index++, song);
        }
        return Task.CompletedTask;
    }

    public Task HandleNotification(SongRemoveNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        SongList?.Remove(notification.RemovedSong);
        return Task.CompletedTask;
    }

    public Task HandleNotification(SongRangeRemovedNotification notification, CancellationToken ctk = new CancellationToken())
    {
        ctk.ThrowIfCancellationRequested();
        if (SongList is null) return Task.CompletedTask;
        foreach (var song in notification.RemovedSongs)
        {
            SongList?.Remove(song);
        }
        return Task.CompletedTask;
    }
}