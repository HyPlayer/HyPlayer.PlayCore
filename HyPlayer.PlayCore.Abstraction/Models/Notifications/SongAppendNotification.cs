using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongAppendNotification : NotificationBase
{
    public SingleSongBase? AppendedSong { get; set; }
}