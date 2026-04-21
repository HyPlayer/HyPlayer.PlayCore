using HyPlayer.PlayCore.Abstraction;

namespace HyPlayer.PlayCore.Wrapper.Notifications
{
    public class PlaylistControllerChangedNotification
    {
        public PlayControllerBase? Controller { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}
