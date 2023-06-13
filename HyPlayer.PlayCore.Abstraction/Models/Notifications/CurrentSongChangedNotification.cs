using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class CurrentSongChangedNotification : NotificationBase
{
    public SingleSongBase? CurrentPlayingSong { get; set; }
}