using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

namespace HyPlayer.PlayCore.Abstraction.Models
{
    public abstract class AudioServiceSettingsBase : IAudioServiceSettings
    {
        public abstract string DefaultOutputDeviceId { get; set; }
        public abstract double OutputVolume { get; set; }
    }
}
