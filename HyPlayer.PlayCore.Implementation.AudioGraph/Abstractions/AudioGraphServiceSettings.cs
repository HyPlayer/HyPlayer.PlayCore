using HyPlayer.PlayCore.Abstraction.Models;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions
{
    public class AudioGraphServiceSettings : AudioServiceSettingsBase
    {
        public override string DefaultOutputDeviceId { get; set; }
        public override double OutputVolume { get; set; }
        internal async Task<AudioGraphSettings> GetAudioGraphSettingsAsync()
        {
            if (!string.IsNullOrEmpty(DefaultOutputDeviceId))
            {
                var deviceInfomation = await DeviceInformation.CreateFromIdAsync(DefaultOutputDeviceId);
                return new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
                {
                    PrimaryRenderDevice = deviceInfomation
                };
            }
            else return new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
        }
    }
}
