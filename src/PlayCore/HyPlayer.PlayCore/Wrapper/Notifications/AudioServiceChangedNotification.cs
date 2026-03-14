using HyPlayer.PlayCore.Abstraction;

namespace HyPlayer.PlayCore.Wrapper.Notifications
{
    public class AudioServiceChangedNotification
    {
        public AudioServiceBase AudioService { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}
