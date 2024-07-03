namespace Abstractions.ServiceManagement;

public interface IServiceDiscovery
{
    TService GetService<TService>() where TService : class;
}