using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongRangeRemovedNotification : NotificationBase
{
    public List<SingleSongBase> RemovedSongs { get; set; }
}