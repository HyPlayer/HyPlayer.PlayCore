using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using HyPlayer.PlayCore.Wrapper.Notifications;
using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Wrapper
{
    public class PlayCoreWrapper : IPlayCoreWrapper
    {
        private readonly HashSet<object> _components = new(); 
        private readonly List<INotificationSubscriberBase> _notificationSubscribers = new();
        private readonly NotificationHub _notificationHub;

        public ObservableCollection<AudioServiceBase> AudioServices 
            { get => new(new ObservableCollection<AudioServiceBase>
                (_components.Where(t => t is AudioServiceBase).Select(t => (AudioServiceBase)t).ToList())); }
        public ObservableCollection<ProviderBase> MusicProviders
            {get => new(new ObservableCollection<ProviderBase>
                (_components.Where(t => t is ProviderBase).Select(t => (ProviderBase)t).ToList()));
        }
        public ObservableCollection<PlayListManagerBase> PlayListControllers
            {
            get => new(new ObservableCollection<PlayListManagerBase>
                (_components.Where(t => t is PlayListManagerBase).Select(t => (PlayListManagerBase)t).ToList()));
        }

        public ObservableCollection<INotificationSubscriberBase> NotificationSubscribers
        { get => new(new ObservableCollection<INotificationSubscriberBase>(_notificationSubscribers.ToList())); }

        public PlayCoreWrapper()
        {
            _notificationHub = new NotificationHub(this);
        }

        public void AddNotificationSubscriber(Type subscriberType)
        {
            _notificationSubscribers.Add((INotificationSubscriberBase) Activator.CreateInstance(subscriberType));
        }

        private object AddCompnentToWrapper(Type type)
        {
            var instance = Activator.CreateInstance(type); 
            _components.Add(instance);
            return instance;
        }

        private void RemoveComponentFromWrapper(Type type)
        {
            _components.Where(t => t.GetType() == type);
        }


        public async void AddAudioService(Type audioServiceType)
        {
            var component = AddCompnentToWrapper(audioServiceType);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = (AudioServiceBase)component,
                ChangeType = 0
            };
            await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification);
        }

        public async void RemoveAudioService(Type audioServiceType)
        {

        }
    }
}
