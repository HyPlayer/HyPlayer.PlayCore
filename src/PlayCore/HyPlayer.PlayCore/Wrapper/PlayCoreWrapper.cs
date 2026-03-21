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

        private AudioServiceBase _currentAudioService;
        private PlayControllerBase _currentPlayController;

        public ObservableCollection<AudioServiceBase> AudioServices
        {
            get => new(new ObservableCollection<AudioServiceBase>
            (_components.Where(t => t is AudioServiceBase).Select(t => (AudioServiceBase)t).ToList()));
        }
        public ObservableCollection<ProviderBase> MusicProviders
        {
            get => new(new ObservableCollection<ProviderBase>
            (_components.Where(t => t is ProviderBase).Select(t => (ProviderBase)t).ToList()));
        }
        public ObservableCollection<PlayListManagerBase> PlayControllers
        {
            get => new(new ObservableCollection<PlayListManagerBase>
                (_components.Where(t => t is PlayListManagerBase).Select(t => (PlayListManagerBase)t).ToList()));
        }

        public ObservableCollection<INotificationSubscriberBase> NotificationSubscribers
        { get => new(new ObservableCollection<INotificationSubscriberBase>(_notificationSubscribers.ToList())); }

        public AudioServiceBase CurrentAudioService => _currentAudioService;
        public PlayControllerBase CurrentPlayController => _currentPlayController;

        public PlayCoreWrapper()
        {
            _notificationHub = new NotificationHub(this);
        }

        public void AddNotificationSubscriber(Type subscriberType)
        {
            _notificationSubscribers.Add((INotificationSubscriberBase)Activator.CreateInstance(subscriberType));
        }

        // 修正方法名拼写（可选，但为了保持兼容性可保留原拼写）
        private object AddCompnentToWrapper(Type type)
        {
            var instance = Activator.CreateInstance(type);
            _components.Add(instance);
            return instance;
        }

        /// <summary>
        /// 从包装器中移除指定类型的所有组件实例
        /// </summary>
        private void RemoveComponentFromWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            _components.RemoveWhere(c => c.GetType() == type);
        }

        // ---------- AudioService 操作 ----------
        public async void AddAudioService(Type audioServiceType)
        {
            var component = AddCompnentToWrapper(audioServiceType);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = (AudioServiceBase)component,
                ChangeType = 0   // 假设 0 表示添加
            };
            await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification);
        }

        public async void RemoveAudioService(Type audioServiceType)
        {
            // 注意：此处未保留被移除的实例引用，通知中只能传 null
            RemoveComponentFromWrapper(audioServiceType);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = null,
                ChangeType = ChangeType.Remove
            };
            await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification);
        }

        // ---------- Provider 操作 ----------
        public async void AddProvider(Type providerType)
        {
            var component = AddCompnentToWrapper(providerType);
            var notification = new ProviderChangeNotification()
            {
                Provider = (ProviderBase)component,
                ChangeType = 0   // 添加
            };
            await _notificationHub.PublishNotificationAsync<ProviderChangeNotification>(notification);
        }

        public async void RemoveProvider(Type providerType)
        {
            RemoveComponentFromWrapper(providerType);
            var notification = new ProviderChangeNotification()
            {
                Provider = null,
                ChangeType = ChangeType.Remove   // 移除
            };
            await _notificationHub.PublishNotificationAsync<ProviderChangeNotification>(notification);
        }

        // ---------- PlayListManager 操作 ----------
        public async void AddPlayController(Type playControllerType)
        {
            var component = AddCompnentToWrapper(playControllerType);
            var notification = new PlaylistControllerChangedNotification()
            {
                Controller = (PlayListManagerBase)component,
                ChangeType = 0   // 添加
            };
            await _notificationHub.PublishNotificationAsync<PlaylistControllerChangedNotification>(notification);
        }

        public async void RemovePlayController(Type playControllerType)
        {
            RemoveComponentFromWrapper(playControllerType);
            var notification = new PlaylistControllerChangedNotification()
            {
                Controller = null,
                ChangeType = ChangeType.Remove   // 移除
            };
            await _notificationHub.PublishNotificationAsync<PlaylistControllerChangedNotification>(notification);
        }

        public void SetCurrentAudioService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            var service = _components.FirstOrDefault(c => c.GetType() == serviceType) as AudioServiceBase;
            if (service == null)
                throw new InvalidOperationException($"未找到类型为 {serviceType.Name} 的 AudioService。");

            _currentAudioService = service;
        }

        public void SetCurrentPlayistController(Type controllerType)
        {
            if (controllerType == null) throw new ArgumentNullException(nameof(controllerType));

            var controller = _components.FirstOrDefault(c => c.GetType() == controllerType) as PlayControllerBase;
            if (controller == null)
                throw new InvalidOperationException($"未找到类型为 {controllerType.Name} 的 PlayListManager。");

            _currentPlayController = controller;
        }
    }
}
