namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlayerModification
{
    public Task SeekAsync(long position);
    public Task PlayAsync();
    public Task PauseAsync();
    public Task StopAsync();
}