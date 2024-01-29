﻿using CommunityToolkit.Mvvm.ComponentModel;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Extensions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    public partial class PositionNotificationSubscriber : ObservableObject, INotificationSubscriber<PlaybackPositionChangedNotification>
    {
        private DispatcherQueue _dispatcherQueue;
        private IDepository _depository;
        public Task HandleNotificationAsync(PlaybackPositionChangedNotification notification, CancellationToken ctk = default)
        {
            if (Sliding) return Task.CompletedTask;
            if (_dispatcherQueue == null)
            {
                _dispatcherQueue = _depository.Resolve<DispatcherQueue>();
            }
            _dispatcherQueue?.TryEnqueue(() =>
            {
                Position = notification.CurrentPlaybackPosition;
            });
            return Task.CompletedTask;
        }
        public PositionNotificationSubscriber(IDepository depository)
        {
            _depository = depository;
        }
        [ObservableProperty]
        private double _position;
        public bool Sliding { get; set; } = false;
    }
    public partial class MasterTicketNotificationSubscriber : ObservableObject, INotificationSubscriber<MasterTicketChangedNotification>
    {
        private DispatcherQueue _dispatcherQueue;
        private IDepository _depository;
        public Task HandleNotificationAsync(MasterTicketChangedNotification notification, CancellationToken ctk = default)
        {
            if (_dispatcherQueue == null)
            {
                _dispatcherQueue = _depository.Resolve<DispatcherQueue>();
            }
            _dispatcherQueue?.TryEnqueue(() =>
            {
                AudioGraphTicket = notification.MasterTicket;
            });
            return Task.CompletedTask;
        }
        public MasterTicketNotificationSubscriber(IDepository depository)
        {
            _depository = depository;
        }
        [ObservableProperty]
        private AudioGraphTicket _audioGraphTicket;
    }
    public partial class OnTicketReachesEndNotificationSubscriber : ObservableObject, INotificationSubscriber<AudioTicketReachesEndNotification>
    {
        private DispatcherQueue _dispatcherQueue;
        private IDepository _depository;
        public Task HandleNotificationAsync(AudioTicketReachesEndNotification notification, CancellationToken ctk = default)
        {
            if (_dispatcherQueue == null)
            {
                _dispatcherQueue = _depository.Resolve<DispatcherQueue>();
            }
            _dispatcherQueue?.TryEnqueue(() =>
            {
                AudioGraphTicketReachesEndName = $"{notification.AudioGraphTicket.MusicResource.ResourceName} Reaches End.";
            });
            return Task.CompletedTask;
        }
        public OnTicketReachesEndNotificationSubscriber(IDepository depository)
        {
            _depository = depository;
        }
        [ObservableProperty]
        private string _audioGraphTicketReachesEndName = string.Empty;
    }
}
