namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCoreService : IPlayCore
{
    public Task RegisterAudioServiceAsync(Type serviceType, Type settingsType,CancellationToken ctk = new());
    public Task RegisterMusicProviderAsync(Type serviceType, CancellationToken ctk = new());
    public Task RegisterPlayControllerAsync(Type serviceType, CancellationToken ctk = new());

    public Task UnregisterAudioServiceAsync(Type serviceType, CancellationToken ctk = new());
    public Task UnregisterMusicProviderAsync(Type serviceType, CancellationToken ctk = new());
    public Task UnregisterPlayControllerAsync(Type serviceType, CancellationToken ctk = new());

    public Task FocusAudioServiceAsync(Type serviceType, CancellationToken ctk = new());
    public Task FocusPlayControllerAsync(Type serviceType, CancellationToken ctk = new());
}