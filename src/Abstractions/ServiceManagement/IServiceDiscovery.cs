namespace Abstractions.ServiceManagement;

/// <summary>
/// Defines the contract for service discovery.
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// Gets the service of the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of the service to get.</typeparam>
    /// <returns>The service of the specified type.</returns>
    TService GetService<TService>() where TService : class;
}
