using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongRangeAppendedNotification : NotificationBase
{
    public required int Index { get; set; }
    public required List<SingleSongBase> AppendedSongs { get; set; }
}