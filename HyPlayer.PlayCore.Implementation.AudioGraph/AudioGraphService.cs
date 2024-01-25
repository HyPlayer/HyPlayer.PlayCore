using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using static HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices.IPlaybackSpeedChangeable;
using Timer = System.Timers.Timer;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService
{
    public class AudioGraphService :
        AudioServiceBase,
        IAudioTicketSeekableService,
        IPlayAudioTicketService,
        IPauseAudioTicketService,
        IStopAudioTicketService,
        IAsyncConstructService,
        IPlaybackRateChangeableService,
        IOutgoingVolumeChangeable,
        IAudioTicketVolumeChangeable,
        IOutputDeviceChangeableService,
        IDisposable
    {
        public override string Id => "com.storyteller.audiograph.chopin";

        public override string Name => "Chopin.AudioGraph";

        public AudioGraph PlaybackAudioGraph { get; internal set; }

        private AudioDeviceOutputNode _deviceOutputNode;

        private AudioGraphSettings _audioGraphSettings;

        private readonly List<AudioGraphTicket> _createdAudioTickets = [];

        private Timer _positionNotifyTimer = new()
        {
            AutoReset = true,
            Interval = 150,
            Enabled = true
        };
        private bool disposedValue;

        public event PlaybackPositionChangedHandler OnPositionChanged;

        public delegate void PlaybackPositionChangedHandler(double Position);

        public AudioGraphTicket MasterTicket { get; internal set; }

        public bool IsPlaybackAudioGraphStarted { get; private set; } = false;

        public override Task DisposeAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            try
            {
                if (audioTicket is AudioGraphTicket ticket)
                {
                    ticket.Dispose();
                }
                return Task.CompletedTask;
            }
            finally
            {
                if (audioTicket is AudioGraphTicket ticket)
                { 
                    if(_createdAudioTickets.Contains(ticket))
                    {
                        _createdAudioTickets.Remove(ticket);
                    }
                }
            }
        }

        public override async Task<AudioTicketBase> GetAudioTicketAsync(MusicResourceBase musicResource, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            var result = await AudioGraphTicket.CreateAudioGraphTicket(musicResource, PlaybackAudioGraph);
            _createdAudioTickets.Add(result);
            return result;
        }

        public override Task<List<AudioTicketBase>> GetCreatedAudioTicketsAsync(CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            var result = _createdAudioTickets.Select(t => t as AudioTicketBase).ToList();
            return Task.FromResult(result);
        }

        public Task PauseAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.Stop();
            }
            return Task.CompletedTask;
        }

        public Task PlayAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                if (ticket.PlaybackMediaSourceInputNode.OutgoingConnections.Count == 0)
                {
                    ConnectTicketToOutputNodeAsync(ticket, _deviceOutputNode);
                }
                ticket.Start();
            }
            return Task.CompletedTask;
        }

        public Task SeekAudioTicketAsync(AudioTicketBase audioTicket, long position, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.PlaybackMediaSourceInputNode.Seek(TimeSpan.FromMilliseconds(position));
            }
            return Task.CompletedTask;
        }

        public async Task StopTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if(audioTicket is AudioGraphTicket ticket)
            {
                ticket.Stop();
                await DisposeAudioTicketAsync(ticket);
            }
        }

        public Task ChangePlaybackSpeedAsync(AudioTicketBase ticket, double playbackSpeed, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if (ticket is AudioGraphTicket graphTicket)
            {
                graphTicket.PlaybackMediaSourceInputNode.PlaybackSpeedFactor = playbackSpeed;
            }
            return Task.CompletedTask;
        }

        public Task ChangeOutgoingVolumeAsync(double volume, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            _deviceOutputNode.OutgoingGain = volume;
            return Task.CompletedTask;
        }
        public async Task<List<OutputDeviceBase>> GetOutputDevicesAsync(CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            var enumeration = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            var outputDevices = new List<OutputDeviceBase>();
            foreach (var device in enumeration)
            {
                outputDevices.Add(new AudioGraphOutputDevice() { DeviceInformation = device, Name = device.Name });
            }
            return outputDevices;
        }

        public async Task SetOutputDevicesAsync(OutputDeviceBase device, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            AudioGraphServiceSettings audioGraphServiceSettings;
            if (device is AudioGraphOutputDevice outputDevice)
            {

                if (outputDevice.DeviceInformation != _deviceOutputNode.Device)
                {
                    audioGraphServiceSettings = new AudioGraphServiceSettings()
                    {
                        AudioGraphSettings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
                        {
                            PrimaryRenderDevice = outputDevice.DeviceInformation
                        }
                    };
                    await CreateAudioGraphFromSettings(audioGraphServiceSettings);
                }

            }
            else
            {
                audioGraphServiceSettings = new AudioGraphServiceSettings()
                {
                    AudioGraphSettings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
                };
                await CreateAudioGraphFromSettings(audioGraphServiceSettings);
            }
            
        }
        private async Task CreateAudioGraphFromSettings(AudioGraphServiceSettings audioGraphServiceSettings)
        {
            var newAudioGraphResult = await AudioGraph.CreateAsync(audioGraphServiceSettings.AudioGraphSettings);
            if (newAudioGraphResult.Status != AudioGraphCreationStatus.Success)
            {
                throw newAudioGraphResult.ExtendedError;
            }
            var newAudioGraph = newAudioGraphResult.Graph;
            var oldAudioGraphCopy = PlaybackAudioGraph;
            foreach (var ticket in _createdAudioTickets)
            {
                if (ticket is AudioGraphTicket audioGraphTicket)
                {
                    audioGraphTicket.Stop();
                    audioGraphTicket.RemoveAllOutputConnections();
                    await audioGraphTicket.ReplaceAudioGraph(newAudioGraph);
                }
            }
            Stop();
            var creationResult = await newAudioGraph.CreateDeviceOutputNodeAsync();
            if (creationResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                throw creationResult.ExtendedError;
            }
            _deviceOutputNode = creationResult.DeviceOutputNode;
            PlaybackAudioGraph = newAudioGraph;
            oldAudioGraphCopy.Dispose();
            await ConnectAllTicketToOutputNodeAsync(_deviceOutputNode);
            Start();
            await PlayAudioTicketAsync(MasterTicket);
        }

        public Task ChangeVolumeAsync(AudioTicketBase ticket, double volume, CancellationToken ctk = default)
        {
            ThrowExceptionIfDisposed();
            ctk.ThrowIfCancellationRequested();
            if (ticket is AudioGraphTicket graphTicket)
            {
                graphTicket.PlaybackMediaSourceInputNode.OutgoingGain = volume;
            }
            return Task.CompletedTask;
        }

        private async Task ConnectAllTicketToOutputNodeAsync(AudioDeviceOutputNode outputNode)
        {
            ThrowExceptionIfDisposed();
            foreach (var ticket in _createdAudioTickets)
            {
                await ConnectTicketToOutputNodeAsync(ticket, outputNode);
            }
        }
        private Task ConnectTicketToOutputNodeAsync(AudioGraphTicket ticket, AudioDeviceOutputNode outputNode)
        {
            ThrowExceptionIfDisposed();
            ticket.PlaybackMediaSourceInputNode.AddOutgoingConnection(outputNode);
            return Task.CompletedTask;
        }
        private void OnPositionNotifyTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ThrowExceptionIfDisposed();
            if (!IsPlaybackAudioGraphStarted || MasterTicket == null)
            {
                return;
            }
            OnPositionChanged?.Invoke(MasterTicket.PlaybackMediaSourceInputNode.Position.TotalMilliseconds);
        }

        public Task SetMasterTicketAsync(AudioGraphTicket graphTicket)
        {
            ThrowExceptionIfDisposed();
            MasterTicket = graphTicket;
            return Task.CompletedTask;
        }

        public void Start()
        {
            ThrowExceptionIfDisposed();
            if (!IsPlaybackAudioGraphStarted)
            {
                PlaybackAudioGraph.Start();
                foreach (var ticket in _createdAudioTickets)
                {
                    ticket.Stop();
                }
                IsPlaybackAudioGraphStarted = true;
            }
        }
        
        public void Stop()
        {
            ThrowExceptionIfDisposed();
            if (IsPlaybackAudioGraphStarted)
            {
                PlaybackAudioGraph.Stop();
                IsPlaybackAudioGraphStarted = false;
            }
        }

        public async Task InitializeService()
        {
            ThrowExceptionIfDisposed();
            var creationResult = await AudioGraph.CreateAsync(_audioGraphSettings);
            if (creationResult.Status != AudioGraphCreationStatus.Success)
            {
                throw creationResult.ExtendedError;
            }
            PlaybackAudioGraph = creationResult.Graph;
            var creationResultDevice = await PlaybackAudioGraph.CreateDeviceOutputNodeAsync();
            if (creationResultDevice.Status != AudioDeviceNodeCreationStatus.Success)
            {
                throw creationResult.ExtendedError;
            }
            _deviceOutputNode = creationResultDevice.DeviceOutputNode;
        }

        public AudioGraphService(AudioServiceSettingsBase serviceSettings)
        {
            if (serviceSettings is AudioGraphServiceSettings settings)
            {
                _audioGraphSettings = settings.AudioGraphSettings;
            }
            _positionNotifyTimer.Elapsed += OnPositionNotifyTimerElapsed;
            _positionNotifyTimer.Start();
        }

        public void ThrowExceptionIfDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(AudioGraphService));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    foreach(var ticket in _createdAudioTickets)
                    {
                        ticket?.Dispose();
                    }
                    _positionNotifyTimer?.Dispose();
                    PlaybackAudioGraph?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
