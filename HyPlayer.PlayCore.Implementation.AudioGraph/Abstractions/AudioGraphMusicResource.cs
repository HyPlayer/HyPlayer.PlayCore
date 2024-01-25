using HyPlayer.PlayCore.Abstraction.Models.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions
{
    public class AudioGraphMusicResource : MusicResourceBase
    {
        public override Task<object> GetResourceAsync(ResourceQualityTag qualityTag = null, Type awaitingType = null, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }
    }
}
