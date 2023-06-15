using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class PlayListChangedNotification : NotificationBase
{
    public required ReadOnlyCollection<SingleSongBase> NewList { get; init; }
    public bool IsRandom { get; init; } = false;
}