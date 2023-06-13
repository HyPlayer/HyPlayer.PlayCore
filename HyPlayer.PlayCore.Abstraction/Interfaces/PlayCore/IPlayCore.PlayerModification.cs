namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlayerModification
{
    public Task Seek(long position);
    public Task Play();
    public Task Pause();
    public Task Stop();
}