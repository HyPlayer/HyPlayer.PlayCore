using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCoreService
{
    public Task RegisterAudioService(Type serviceType);
    public Task RegisterMusicProvider(Type serviceType);
    public Task RegisterPlayListController(Type serviceType);

    public Task UnregisterAudioService(Type serviceType);
    public Task UnregisterMusicProvider(Type serviceType);
    public Task UnregisterPlayListController(Type serviceType);

    public Task FocusAudioService(Type serviceType);
    public Task FocusPlayListController(Type serviceType);
}