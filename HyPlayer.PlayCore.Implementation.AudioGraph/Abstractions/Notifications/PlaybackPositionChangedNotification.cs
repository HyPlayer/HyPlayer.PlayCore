using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications
{
    public class PlaybackPositionChangedNotification
    {
        public double CurrentPlaybackPosition { get; init; }
        public PlaybackPositionChangedNotification(double currentPlaybackPosition)
        {
            CurrentPlaybackPosition = currentPlaybackPosition;
        }
    }
}
