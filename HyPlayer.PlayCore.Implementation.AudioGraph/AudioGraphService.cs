﻿using Depository.Abstraction.Interfaces;
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
        IOutputDeviceChangeableService
    {
        public override string Id => "com.storyteller.audiograph.chopin";

        public override string Name => "Chopin.AudioGraph";

        public AudioGraph PlaybackAudioGraph { get; internal set; }

        private AudioDeviceOutputNode _deviceOutputNode;

        private AudioGraphSettings _audioGraphSettings;

        private List<AudioGraphTicket> _createdAudioTickets = [];

        private Timer _positionNotifyTimer = new()
        {
            AutoReset = true,
            Interval = 150,
            Enabled = true
        };

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
                    _createdAudioTickets.Remove(ticket);
            }
        }

        public override async Task<AudioTicketBase> GetAudioTicketAsync(MusicResourceBase musicResource, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            var result = await AudioGraphTicket.CreateAudioGraphTicket(musicResource, PlaybackAudioGraph);
            await ConnectTicketToOutputNodeAsync(result, _deviceOutputNode);
            _createdAudioTickets.Add(result);
            return result;
        }

        public override Task<List<AudioTicketBase>> GetCreatedAudioTicketsAsync(CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            var result = _createdAudioTickets.Select(t => t as AudioTicketBase).ToList();
            return Task.FromResult(result);
        }

        public Task PauseAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.Stop();
            }
            return Task.CompletedTask;
        }

        public Task PlayAudioTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            Start();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.Start();
            }
            return Task.CompletedTask;
        }

        public Task SeekAudioTicketAsync(AudioTicketBase audioTicket, long position, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.PlaybackMediaSourceInputNode.Seek(TimeSpan.FromMilliseconds(position));
            }
            return Task.CompletedTask;
        }

        public Task StopTicketAsync(AudioTicketBase audioTicket, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            if (audioTicket is AudioGraphTicket ticket)
            {
                ticket.Stop();
            }
            Stop();
            return Task.CompletedTask;
        }

        public Task ChangePlaybackSpeedAsync(AudioTicketBase ticket, double playbackSpeed, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            if (ticket is AudioGraphTicket graphTicket)
            {
                graphTicket.PlaybackMediaSourceInputNode.PlaybackSpeedFactor = playbackSpeed;
            }
            return Task.CompletedTask;
        }

        public Task ChangeOutgoingVolumeAsync(double volume, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            _deviceOutputNode.OutgoingGain = volume;
            return Task.CompletedTask;
        }
        public async Task<List<OutputDeviceBase>> GetOutputDevicesAsync(CancellationToken ctk = default)
        {
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
            ctk.ThrowIfCancellationRequested();
            if (device is AudioGraphOutputDevice outputDevice)
            {
                if (outputDevice.DeviceInformation != _deviceOutputNode.Device)
                {
                    var audioGraphServiceSettings = new AudioGraphServiceSettings()
                    {
                        AudioGraphSettings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
                        {
                            PrimaryRenderDevice = outputDevice.DeviceInformation
                        }
                    };
                    var newAudioGraphResult = await AudioGraph.CreateAsync(audioGraphServiceSettings.AudioGraphSettings);
                    if (newAudioGraphResult.Status != AudioGraphCreationStatus.Success)
                    {
                        throw newAudioGraphResult.ExtendedError;
                    }
                    var newAudioGraph = newAudioGraphResult.Graph;
                    var oldAudioGraphCopy = PlaybackAudioGraph;
                    var oldDeviceOutputNode = _deviceOutputNode;
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
                    oldDeviceOutputNode.Dispose();
                    await ConnectAllTicketToOutputNodeAsync(_deviceOutputNode);
                    Start();
                }
            }
        }

        public Task ChangeVolumeAsync(AudioTicketBase ticket, double volume, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            if (ticket is AudioGraphTicket graphTicket)
            {
                graphTicket.PlaybackMediaSourceInputNode.OutgoingGain = volume;
            }
            return Task.CompletedTask;
        }

        private async Task ConnectAllTicketToOutputNodeAsync(AudioDeviceOutputNode outputNode)
        {
            foreach (var ticket in _createdAudioTickets)
            {
                await ConnectTicketToOutputNodeAsync(ticket, outputNode);
            }
        }
        private Task ConnectTicketToOutputNodeAsync(AudioGraphTicket ticket, AudioDeviceOutputNode outputNode)
        {
            ticket.PlaybackMediaSourceInputNode.AddOutgoingConnection(outputNode);
            return Task.CompletedTask;
        }
        private void OnPositionNotifyTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsPlaybackAudioGraphStarted || MasterTicket == null)
            {
                return;
            }
            OnPositionChanged?.Invoke(MasterTicket.PlaybackMediaSourceInputNode.Position.TotalMilliseconds);
        }

        public void Start()
        {

            if (!IsPlaybackAudioGraphStarted)
            {
                PlaybackAudioGraph.Start();
                _deviceOutputNode.Start();
                IsPlaybackAudioGraphStarted = true;
            }
        }

        public void Stop()
        {
            if (IsPlaybackAudioGraphStarted)
            {
                PlaybackAudioGraph.Stop();
                _deviceOutputNode.Stop();
                IsPlaybackAudioGraphStarted = false;
            }
        }

        public async Task InitializeService()
        {
            var createResult = await AudioGraph.CreateAsync(_audioGraphSettings);
            if (createResult.Status != AudioGraphCreationStatus.Success)
            {
                throw createResult.ExtendedError;
            }
            PlaybackAudioGraph = createResult.Graph;
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
    }
}
