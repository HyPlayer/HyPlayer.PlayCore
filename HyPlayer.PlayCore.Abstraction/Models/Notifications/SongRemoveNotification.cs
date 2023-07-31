using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongRemoveNotification : NotificationBase
{
    public required SingleSongBase RemovedSong { get; set; }
}