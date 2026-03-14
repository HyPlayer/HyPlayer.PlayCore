namespace HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub
{
    public interface INotificationSubscriberBase
    {

    }

    public interface INotificationSubscriber<in TNotification> : INotificationSubscriberBase
    {
        public Task HandleNotificationAsync(TNotification notification, CancellationToken ctk = new());
    }

    public interface INotificationSubscriber<in TNotification, TResult> : INotificationSubscriberBase
    {
        public Task<TResult> HandleNotificationAsync(TNotification notification, CancellationToken ctk = new());
    }
}
