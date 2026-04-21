using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using HyPlayer.PlayCore.Abstraction.Models;
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
        private INotificationHub? _notificationHub;

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
        }

        // Initialize the wrapper with a notification hub after both singletons are created to avoid
        // circular constructor dependencies when using DI containers that construct singletons eagerly.
        public void Initialize(INotificationHub notificationHub)
        {
            _notificationHub = notificationHub ?? throw new ArgumentNullException(nameof(notificationHub));
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
        public async Task AddAudioService(Type audioServiceType, Type audioServiceSettingsType)
        {
            if (audioServiceType == null) throw new ArgumentNullException(nameof(audioServiceType));
            if (!typeof(AudioServiceBase).IsAssignableFrom(audioServiceType)) throw new ArgumentException("Type must derive from AudioServiceBase", nameof(audioServiceType));

            if (audioServiceSettingsType == null) throw new ArgumentNullException(nameof(audioServiceSettingsType));

            // Prefer to instantiate the settings type from the same assembly as the service type
            var settingsTypeToCreate = audioServiceType.Assembly.GetType(audioServiceSettingsType.FullName) ?? audioServiceSettingsType;
            var settingsInstance = Activator.CreateInstance(settingsTypeToCreate) ?? throw new InvalidOperationException($"Unable to create instance of {settingsTypeToCreate.FullName}");

            await AddAudioService(audioServiceType, settingsInstance).ConfigureAwait(false);
        }

        public async Task AddAudioService(Type audioServiceType, object audioServiceSettingsInstance)
        {
            if (audioServiceType == null) throw new ArgumentNullException(nameof(audioServiceType));
            if (!typeof(AudioServiceBase).IsAssignableFrom(audioServiceType)) throw new ArgumentException("Type must derive from AudioServiceBase", nameof(audioServiceType));
            if (audioServiceSettingsInstance == null) throw new ArgumentNullException(nameof(audioServiceSettingsInstance));

            // Find a constructor that can accept the provided settings instance and the notification hub (or a compatible type).
            // Use IsAssignableFrom for robust assignability checks and fall back to creating a settings instance
            // from the service assembly when types have the same FullName but come from different load contexts.
            var ctors = audioServiceType.GetConstructors();
            System.Reflection.ConstructorInfo? selectedCtor = null;
            object? ctorSettingsInstance = null;

            var providedSettingsType = audioServiceSettingsInstance.GetType();

            foreach (var ctor in ctors)
            {
                var ps = ctor.GetParameters();
                if (ps.Length == 2)
                {
                    var p0 = ps[0].ParameterType;
                    var p1 = ps[1].ParameterType;

                    // Direct assignable check
                    if (p0.IsAssignableFrom(providedSettingsType) &&
                        (_notificationHub == null || p1.IsAssignableFrom(_notificationHub.GetType())))
                    {
                        selectedCtor = ctor;
                        ctorSettingsInstance = audioServiceSettingsInstance;
                        break;
                    }

                    // Fallback: same FullName but different assembly/context -> try to create settings from service assembly
                    if (p0.FullName == providedSettingsType.FullName)
                    {
                        var targetSettingsType = audioServiceType.Assembly.GetType(p0.FullName);
                        if (targetSettingsType != null)
                        {
                            try
                            {
                                var created = Activator.CreateInstance(targetSettingsType);
                                if (created != null && p0.IsAssignableFrom(created.GetType()))
                                {
                                    selectedCtor = ctor;
                                    ctorSettingsInstance = created;
                                    break;
                                }
                            }
                            catch
                            {
                                // ignore and continue searching
                            }
                        }
                    }
                }
                else if (ps.Length == 1)
                {
                    var p0 = ps[0].ParameterType;
                    if (p0.IsAssignableFrom(providedSettingsType))
                    {
                        selectedCtor = ctor;
                        ctorSettingsInstance = audioServiceSettingsInstance;
                        break;
                    }

                    if (p0.FullName == providedSettingsType.FullName)
                    {
                        var targetSettingsType = audioServiceType.Assembly.GetType(p0.FullName);
                        if (targetSettingsType != null)
                        {
                            try
                            {
                                var created = Activator.CreateInstance(targetSettingsType);
                                if (created != null && p0.IsAssignableFrom(created.GetType()))
                                {
                                    selectedCtor = ctor;
                                    ctorSettingsInstance = created;
                                    break;
                                }
                            }
                            catch
                            {
                                // ignore and continue searching
                            }
                        }
                    }
                }
            }

            if (selectedCtor == null || ctorSettingsInstance == null)
            {
                throw new MissingMethodException($"No compatible constructor found for {audioServiceType.FullName} that accepts the provided settings instance and notification hub.");
            }

            object component;
            var parameters = selectedCtor.GetParameters();
            if (parameters.Length == 2)
            {
                component = selectedCtor.Invoke(new object[] { ctorSettingsInstance, _notificationHub });
            }
            else
            {
                component = selectedCtor.Invoke(new object[] { ctorSettingsInstance });
            }

            _components.Add(component);
            var notification = new AudioServiceChangedNotification()
            {
                AudioService = (AudioServiceBase)component,
                ChangeType = 0   // 假设 0 表示添加
            };
            if (_notificationHub != null)
            {
                await _notificationHub.PublishNotificationAsync<AudioServiceChangedNotification>(notification).ConfigureAwait(false);
            }
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
            if (_notificationHub != null)
            {
                await _notificationHub.PublishNotificationAsync<ProviderChangeNotification>(notification).ConfigureAwait(false);
            }
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
            if (!typeof(PlayControllerBase).IsAssignableFrom(playControllerType)) throw new ArgumentException("Type must derive from PlayListManagerBase", nameof(playControllerType));
            var component = AddComponentToWrapper(playControllerType);
            var notification = new PlaylistControllerChangedNotification()
            {
                Controller = (PlayControllerBase) component,
                ChangeType = 0   // 添加
            };
            if (_notificationHub != null)
            {
                await _notificationHub.PublishNotificationAsync<PlaylistControllerChangedNotification>(notification).ConfigureAwait(false);
            }
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
