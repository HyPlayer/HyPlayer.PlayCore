using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HyPlayer.PlayCore.Freud
{
    public class ServiceCollection
    {
        private readonly List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();
        // Register a singleton by implementation type with parameterless constructor (no reflection used internally)
        public void AddSingleton<TService, TImpl>() where TImpl : TService, new()
        {
            _descriptors.Add(new ServiceDescriptor(typeof(TService), sp => (object)new TImpl(), ServiceLifetime.Singleton));
        }

        // Register a singleton instance
        public void AddSingleton<TService>(TService instance)
        {
            _descriptors.Add(new ServiceDescriptor(typeof(TService), (object)instance));
        }

        // Register a singleton with factory
        public void AddSingleton<TService>(Func<IServiceProvider, TService> factory)
        {
            _descriptors.Add(new ServiceDescriptor(typeof(TService), sp => (object)factory(sp), ServiceLifetime.Singleton));
        }

        // Register a transient by implementation type with parameterless constructor
        public void AddTransient<TService, TImpl>() where TImpl : TService, new()
        {
            _descriptors.Add(new ServiceDescriptor(typeof(TService), sp => (object)new TImpl(), ServiceLifetime.Transient));
        }

        // Register a transient with factory
        public void AddTransient<TService>(Func<IServiceProvider, TService> factory)
        {
            _descriptors.Add(new ServiceDescriptor(typeof(TService), sp => (object)factory(sp), ServiceLifetime.Transient));
        }

        // Build and return an IServiceProvider compatible provider.
        public IServiceProvider BuildServiceProvider()
        {
            return new ServiceProvider(_descriptors);
        }

        internal IReadOnlyList<ServiceDescriptor> Descriptors => _descriptors.AsReadOnly();
    }

    public class ServiceProvider : IServiceProvider, IDisposable
    {
        private readonly IReadOnlyList<ServiceDescriptor> _descriptors;
        private readonly ConcurrentDictionary<Type, object> _singletonInstances = new ConcurrentDictionary<Type, object>();
        private readonly Stack<Type> _callStack = new Stack<Type>();
        private bool _disposed;

        internal ServiceProvider(IReadOnlyList<ServiceDescriptor> descriptors)
        {
            _descriptors = descriptors ?? throw new ArgumentNullException(nameof(descriptors));
            // Preload existing singleton instances from descriptors
            foreach (var d in _descriptors.Where(x => x.Lifetime == ServiceLifetime.Singleton && x.ImplementationInstance != null))
            {
                _singletonInstances[d.ServiceType] = d.ImplementationInstance;
            }
        }

        public object GetService(Type serviceType)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ServiceProvider));

            var descriptor = _descriptors.FirstOrDefault(d => d.ServiceType == serviceType);
            if (descriptor == null)
            {
                throw new InvalidOperationException($"Service of type {serviceType.FullName} is not registered");
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                return _singletonInstances.GetOrAdd(serviceType, _ => CreateInstance(descriptor));
            }

            return CreateInstance(descriptor);
        }

        private object CreateInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationFactory != null)
            {
                // simple circular detection based on service type stack
                if (_callStack.Contains(descriptor.ServiceType))
                {
                    var cycle = string.Join(" -> ", _callStack.Reverse().Select(t => t.Name).Concat(new[] { descriptor.ServiceType.Name }));
                    throw new InvalidOperationException($"Circular dependency detected: {cycle}");
                }

                _callStack.Push(descriptor.ServiceType);
                try
                {
                    var instance = descriptor.ImplementationFactory(this);
                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        _singletonInstances[descriptor.ServiceType] = instance;
                    }
                    return instance;
                }
                finally
                {
                    _callStack.Pop();
                }
            }

            throw new InvalidOperationException("Invalid service descriptor: no factory or instance provided");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var kv in _singletonInstances)
            {
                if (kv.Value is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }
    }
}
