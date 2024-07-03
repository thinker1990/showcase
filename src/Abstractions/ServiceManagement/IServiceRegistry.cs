namespace Abstractions.ServiceManagement;

public interface IServiceRegistry
{
    void Register<TService, TImplementation>(TImplementation service)
        where TService : class
        where TImplementation : class, TService;
}