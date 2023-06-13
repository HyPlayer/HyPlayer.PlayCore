﻿using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore;

public partial class Chopin
{
    public override async Task MovePointerTo(SingleSongBase song)
    {
        if (CurrentPlayListController is { } controller)
        {
            await controller.ChangePlayingTo(song);
        }
    }

    public override async Task MoveNext()
    {
        if (CurrentPlayListController is { } controller)
        {
            await controller.MoveNext();
        }
    }

    public override async Task MovePrevious()
    {
        if (CurrentPlayListController is { } controller)
        {
            await controller.MovePrevious();
        }
    }
}