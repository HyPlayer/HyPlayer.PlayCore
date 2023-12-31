﻿namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IOutgoingVolumeChangeable : IAudioService
{
    public Task ChangeOutgoingVolumeAsync(double volume, CancellationToken ctk = new());
}