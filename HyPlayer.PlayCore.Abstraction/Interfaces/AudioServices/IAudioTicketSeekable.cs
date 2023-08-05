﻿using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

/// <summary>
/// 允许进行进度控制
/// </summary>
public interface IAudioTicketSeekableService : IAudioService
{
    public Task SeekAudioTicket(AudioTicketBase audioTicket, long position, CancellationToken ctk = new());
}