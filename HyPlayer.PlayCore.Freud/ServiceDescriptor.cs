using System;

namespace HyPlayer.PlayCore.Freud
{
    public enum ServiceLifetime
    {
        Singleton,
        Transient
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Func<IServiceProvider, object> ImplementationFactory { get; }
        public object ImplementationInstance { get; internal set; }
        public ServiceLifetime Lifetime { get; }

        public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            Lifetime = lifetime;
        }

        public ServiceDescriptor(Type serviceType, object instance)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationInstance = instance ?? throw new ArgumentNullException(nameof(instance));
            Lifetime = ServiceLifetime.Singleton;
        }
    }
}
