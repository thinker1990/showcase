using Abstractions.ServiceManagement;

namespace Infrastructure.ServiceManagement;

/// <summary>
/// Represents a container for managing service registrations and discovery.
/// </summary>
public sealed class ServiceContainer : IServiceRegistry, IServiceDiscovery
{
    private readonly Dictionary<Type, object> _registry = [];

    /// <summary>
    /// Registers a service with its implementation.
    /// </summary>
    /// <typeparam name="TService">The type of the service to register.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service.</typeparam>
    /// <param name="service">The implementation of the service to register.</param>
    /// <exception cref="EntityDuplicateException">Thrown when the service is already registered.</exception>
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

    /// <summary>
    /// Gets the service of the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of the service to get.</typeparam>
    /// <returns>The service of the specified type.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the service is not found.</exception>
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
