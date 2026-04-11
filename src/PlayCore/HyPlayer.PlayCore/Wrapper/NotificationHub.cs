using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Wrapper
{
    public class NotificationHub : INotificationHub
    {
        private readonly PlayCoreWrapper _wrapper;

        public NotificationHub(PlayCoreWrapper wrapper)
        {
            _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        }

        public async Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = default)
        {
            var subscribers = _wrapper.NotificationSubscribers
                .OfType<INotificationSubscriber<TNotification>>()
                .ToList();

            var tasks = subscribers
                .Select(handler => handler.HandleNotificationAsync(notification, ctk))
                .ToArray();

            if (tasks.Length > 0)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public async Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification, CancellationToken ctk = default)
        {
            var subscribers = _wrapper.NotificationSubscribers
                .OfType<INotificationSubscriber<TNotification, TResult>>()
                .ToList();
            var results = new List<TResult>();
            foreach (var subscriber in subscribers)
            {
                try
                {
                    results.Add(await subscriber.HandleNotificationAsync(notification, ctk).ConfigureAwait(false));
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return results;
        }
    }
}
