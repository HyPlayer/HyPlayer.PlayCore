using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper
{
    public interface IPlayCoreWrapper
    {
        ObservableCollection<AudioServiceBase> AudioServices { get; }
        ObservableCollection<ProviderBase> MusicProviders { get;}
        ObservableCollection<PlayListManagerBase> PlayControllers { get; }

        AudioServiceBase? CurrentAudioService { get; }
        PlayControllerBase? CurrentPlayController { get; }

        void AddNotificationSubscriber(Type subscriberType);
        Task AddAudioService(Type audioServiceType);
        Task RemoveAudioService(Type audioServiceType);
        Task AddProvider(Type providerType);
        Task RemoveProvider(Type providerType);
        Task AddPlayController(Type playControllerType);
        Task RemovePlayController(Type playControllerType);
        void SetCurrentAudioService(Type serviceType);
        void SetCurrentPlayistController(Type serviceType);
    }
}
