﻿using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications;

public class SongAppendNotification : NotificationBase
{
    public required SingleSongBase AppendedSong { get; set; }
    public required int Index { get; set; }
}