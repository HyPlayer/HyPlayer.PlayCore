using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongAppendNotification : NotificationBase
{
    public required SingleSongBase AppendedSong { get; set; }
    public required int Index { get; set; }
}