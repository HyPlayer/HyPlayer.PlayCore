namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IOutgoingVolumeChangeable
{
    public Task ChangeOutgoingVolume(double volume);
}