namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications
{
    public class PlaybackPositionChangedNotification
    {
        public double CurrentPlaybackPosition { get; init; }
        public PlaybackPositionChangedNotification(double currentPlaybackPosition)
        {
            CurrentPlaybackPosition = currentPlaybackPosition;
        }
    }
}
