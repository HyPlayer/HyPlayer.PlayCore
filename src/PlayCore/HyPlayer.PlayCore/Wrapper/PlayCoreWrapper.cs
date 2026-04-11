using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using HyPlayer.PlayCore.Wrapper.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Wrapper
{
    public class PlayCoreWrapper : IPlayCoreWrapper
    {
        private readonly HashSet<object> _components = new();
        private readonly List<INotificationSubscriberBase> _notificationSubscribers = new();
        private readonly NotificationHub _notificationHub;

        private AudioServiceBase? _currentAudioService;
        private PlayControllerBase? _currentPlayController;

        public ObservableCollection<AudioServiceBase> AudioServices
        {
            get => new ObservableCollection<AudioServiceBase>(_components.OfType<AudioServiceBase>().ToList());
        }
        public ObservableCollection<ProviderBase> MusicProviders
        {
            get => new ObservableCollection<ProviderBase>(_components.OfType<ProviderBase>().ToList());
        }
        public ObservableCollection<PlayListManagerBase> PlayControllers
        {
            get => new ObservableCollection<PlayListManagerBase>(_components.OfType<PlayListManagerBase>().ToList());
        }

        public ObservableCollection<INotificationSubscriberBase> NotificationSubscribers
        { get => new ObservableCollection<INotificationSubscriberBase>(_notificationSubscribers.ToList()); }

        public AudioServiceBase? CurrentAudioService => _currentAudioService;
        public PlayControllerBase? CurrentPlayController => _currentPlayController;

        public PlayCoreWrapper()
        {
            _notificationHub = new NotificationHub(this);
        }

        public void AddNotificationSubscriber(Type subscriberType)
        {
            if (subscriberType == null) throw new ArgumentNullException(nameof(subscriberType));
            var instance = Activator.CreateInstance(subscriberType);
            if (instance is INotificationSubscriberBase sub)
            {
                _notificationSubscribers.Add(sub);
            }
            else
            {
                throw new InvalidOperationException($"Type {subscriberType.FullName} does not implement INotificationSubscriberBase");
            }
        }

        // 创建并添加组件到包装器，返回实例（安全检查）
        private object AddComponentToWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Unable to create instance of {type.FullName}");
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
        public async Task AddAudioService(Type audioServiceType)
        {
            if (audioServiceType == null) throw new ArgumentNullException(nameof(audioServiceType));
            if (!typeof(AudioServiceBase).IsAssignableFrom(audioServiceType)) throw new ArgumentException("Type must derive from AudioServiceBase", nameof(audioServiceType));
            var component = AddComponentToWrapper(audioServiceType);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = (AudioServiceBase)component,
                ChangeType = 0   // 假设 0 表示添加
            };
            await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification).ConfigureAwait(false);
        }

        public async Task RemoveAudioService(Type audioServiceType)
        {
            if (audioServiceType == null) throw new ArgumentNullException(nameof(audioServiceType));
            RemoveComponentFromWrapper(audioServiceType);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = null,
                ChangeType = ChangeType.Remove
            };
            await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification).ConfigureAwait(false);
        }

        // ---------- Provider 操作 ----------
        public async Task AddProvider(Type providerType)
        {
            if (providerType == null) throw new ArgumentNullException(nameof(providerType));
            if (!typeof(ProviderBase).IsAssignableFrom(providerType)) throw new ArgumentException("Type must derive from ProviderBase", nameof(providerType));
            var component = AddComponentToWrapper(providerType);
            var notification = new ProviderChangeNotification()
            {
                Provider = (ProviderBase)component,
                ChangeType = 0   // 添加
            };
            await _notificationHub.PublishNotificationAsync<ProviderChangeNotification>(notification).ConfigureAwait(false);
        }

        public async Task RemoveProvider(Type providerType)
        {
            if (providerType == null) throw new ArgumentNullException(nameof(providerType));
            RemoveComponentFromWrapper(providerType);
            var notification = new ProviderChangeNotification()
            {
                Provider = null,
                ChangeType = ChangeType.Remove   // 移除
            };
            await _notificationHub.PublishNotificationAsync<ProviderChangeNotification>(notification).ConfigureAwait(false);
        }

        // ---------- PlayListManager 操作 ----------
        public async Task AddPlayController(Type playControllerType)
        {
            if (playControllerType == null) throw new ArgumentNullException(nameof(playControllerType));
            if (!typeof(PlayListManagerBase).IsAssignableFrom(playControllerType)) throw new ArgumentException("Type must derive from PlayListManagerBase", nameof(playControllerType));
            var component = AddComponentToWrapper(playControllerType);
            var notification = new PlaylistControllerChangedNotification()
            {
                Controller = (PlayListManagerBase)component,
                ChangeType = 0   // 添加
            };
            await _notificationHub.PublishNotificationAsync<PlaylistControllerChangedNotification>(notification).ConfigureAwait(false);
        }

        public async Task RemovePlayController(Type playControllerType)
        {
            if (playControllerType == null) throw new ArgumentNullException(nameof(playControllerType));
            RemoveComponentFromWrapper(playControllerType);
            var notification = new PlaylistControllerChangedNotification()
            {
                Controller = null,
                ChangeType = ChangeType.Remove   // 移除
            };
            await _notificationHub.PublishNotificationAsync<PlaylistControllerChangedNotification>(notification).ConfigureAwait(false);
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
