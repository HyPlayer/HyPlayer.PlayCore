using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCoreService
{
    public Task RegisterAudioServiceAsync(Type serviceType);
    public Task RegisterMusicProviderAsync(Type serviceType);
    public Task RegisterPlayListControllerAsync(Type serviceType);

    public Task UnregisterAudioServiceAsync(Type serviceType);
    public Task UnregisterMusicProviderAsync(Type serviceType);
    public Task UnregisterPlayListControllerAsync(Type serviceType);

    public Task FocusAudioServiceAsync(Type serviceType);
    public Task FocusPlayListControllerAsync(Type serviceType);
}