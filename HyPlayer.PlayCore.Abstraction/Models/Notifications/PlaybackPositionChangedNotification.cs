namespace HyPlayer.PlayCore.Abstraction.Models.Notifications
{
    public abstract class PlaybackPositionChangedNotification
    {
        public abstract double CurrentPlaybackPosition { get; init; }
    }
}
