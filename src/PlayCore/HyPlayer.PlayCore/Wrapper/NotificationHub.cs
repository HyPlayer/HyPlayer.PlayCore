using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyPlayer.PlayCore.Wrapper
{
    public class NotificationHub : INotificationHub
    {
        public NotificationHub()
        {

        }

        public Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }
    }
}
