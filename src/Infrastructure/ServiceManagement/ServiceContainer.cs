using Abstractions.ServiceManagement;

namespace Infrastructure.ServiceManagement;

public sealed class ServiceContainer : IServiceRegistry, IServiceDiscovery
{
    private readonly Dictionary<Type, object> _registry = [];

    public void Register<TService, TImplementation>(TImplementation service)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        if (!_registry.TryAdd(serviceType, service))
        {
            throw new EntityDuplicateException(serviceType.Name);
        }
    }

    public TService GetService<TService>()
        where TService : class
    {
        var serviceType = typeof(TService);
        if (!_registry.ContainsKey(serviceType))
        {
            throw new EntityNotFoundException(serviceType.Name);
        }

        return (TService)_registry[serviceType];
    }
}