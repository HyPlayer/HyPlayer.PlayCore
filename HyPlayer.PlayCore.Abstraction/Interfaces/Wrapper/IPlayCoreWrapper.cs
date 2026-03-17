using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper
{
    public interface IPlayCoreWrapper
    {
        ObservableCollection<AudioServiceBase> AudioServices { get; }
        ObservableCollection<ProviderBase> MusicProviders { get;}
        ObservableCollection<PlayListManagerBase> PlayListControllers { get; }

        AudioServiceBase CurrentAudioService { get; }
        ProviderBase CurrentMusicProvider { get; }
        PlayListManagerBase CurrentPlayListController { get; }

        void AddNotificationSubscriber(Type subscriberType);
        void AddAudioService(Type audioServiceType);
        void RemoveAudioService(Type audioServiceType);
        void AddProvider(Type providerType);
        void RemoveProvider(Type providerType);
        void AddPlayListManager(Type playListManagerType);
        void RemovePlayListManager(Type playListManagerType);
        void SetCurrentAudioService(Type serviceType);
        void SetCurrentProvider(Type serviceType);
        void SetCurrentPlayistController(Type serviceType);
    }
}
