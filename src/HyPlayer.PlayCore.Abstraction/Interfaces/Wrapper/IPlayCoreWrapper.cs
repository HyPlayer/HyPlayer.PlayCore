using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper
{
    public interface IPlayCoreWrapper
    {
        ObservableCollection<AudioServiceBase> AudioServices { get; }
        ObservableCollection<ProviderBase> MusicProviders { get;}
        ObservableCollection<PlayListManagerBase> PlayControllers { get; }

        ObservableCollection<INotificationSubscriberBase> NotificationSubscribers { get; }

        AudioServiceBase? CurrentAudioService { get; }
        PlayControllerBase? CurrentPlayController { get; }

        void AddNotificationSubscriber(Type subscriberType);
        Task AddAudioService(Type audioServiceType, Type audioServiceSettingsType);
        // Overload that accepts an already-created settings instance to avoid assembly/type identity
        // issues when the settings type may be loaded from a different assembly/load context.
        Task AddAudioService(Type audioServiceType, object audioServiceSettingsInstance);
        Task RemoveAudioService(Type audioServiceType);
        Task AddProvider(Type providerType);
        Task RemoveProvider(Type providerType);
        Task AddPlayController(Type playControllerType);
        Task RemovePlayController(Type playControllerType);
        void SetCurrentAudioService(Type serviceType);
        void SetCurrentPlayistController(Type serviceType);
    }
}
