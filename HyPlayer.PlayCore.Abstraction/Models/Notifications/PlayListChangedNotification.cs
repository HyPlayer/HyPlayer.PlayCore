using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class PlayListChangedNotification : NotificationBase
{
    public required List<SingleSongBase> NewList { get; init; }
    public bool IsRandom { get; set; } = false;
}