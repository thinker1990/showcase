namespace Abstractions.ServiceManagement;

/// <summary>
/// Defines the contract for a service registry.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Registers a service with its implementation.
    /// </summary>
    /// <typeparam name="TService">The type of the service to register.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service.</typeparam>
    /// <param name="service">The implementation of the service to register.</param>
    void Register<TService, TImplementation>(TImplementation service)
        where TService : class
        where TImplementation : class, TService;
}
