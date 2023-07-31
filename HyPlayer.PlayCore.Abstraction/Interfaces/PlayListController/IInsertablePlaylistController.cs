﻿using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IInsertablePlaylistController
{
    public abstract Task InsertSong(SingleSongBase song, int index);
    public abstract Task RemoveSong(SingleSongBase song);
}