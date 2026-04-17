using HyPlayer.PlayCore.Abstraction;

namespace HyPlayer.PlayCore.Wrapper.Notifications
{
    public class ProviderChangeNotification
    {
        public ProviderBase? Provider { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}
