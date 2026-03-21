using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper
{
    public interface IPlayCoreWrapper
    {
        ObservableCollection<AudioServiceBase> AudioServices { get; }
        ObservableCollection<ProviderBase> MusicProviders { get;}
        ObservableCollection<PlayListManagerBase> PlayControllers { get; }

        AudioServiceBase CurrentAudioService { get; }
        PlayControllerBase CurrentPlayController { get; }

        void AddNotificationSubscriber(Type subscriberType);
        void AddAudioService(Type audioServiceType);
        void RemoveAudioService(Type audioServiceType);
        void AddProvider(Type providerType);
        void RemoveProvider(Type providerType);
        void AddPlayController(Type playControllerType);
        void RemovePlayController(Type playControllerType);
        void SetCurrentAudioService(Type serviceType);
        void SetCurrentPlayistController(Type serviceType);
    }
}
