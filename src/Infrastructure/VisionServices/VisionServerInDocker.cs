using Abstractions.VisionServices;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Infrastructure.VisionServices;

public sealed class VisionServerInDocker(Uri endpoint) : IVisionServer, IDisposable
{
    private readonly DockerClient _client = new DockerClientConfiguration(endpoint).CreateClient();

    public string Host => endpoint.Host;

    public Task<IEnumerable<IVisionService>> Services => ListContainers();

    private async Task<IEnumerable<IVisionService>> ListContainers()
    {
        var parameter = new ContainersListParameters { All = true };
        var containers = await _client.Containers.ListContainersAsync(parameter);

        return containers.Select(it => new VisionServiceInDocker(it, _client));
    }

    public void Dispose() => _client.Dispose();
}