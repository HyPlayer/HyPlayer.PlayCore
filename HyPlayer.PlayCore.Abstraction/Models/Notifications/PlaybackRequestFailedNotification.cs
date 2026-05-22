using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class PlaybackRequestFailedNotification : NotificationBase
{
    public SingleSongBase? Song { get; init; }
    public required string Reason { get; init; }
    public Exception? Exception { get; init; }
}
