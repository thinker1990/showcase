using Abstractions.VisionServices;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Infrastructure.VisionServices;

/// <summary>
/// Represents a vision server running in a Docker container.
/// </summary>
/// <param name="endpoint">The URI endpoint of the Docker server.</param>
public sealed class VisionServerInDocker(Uri endpoint) : IVisionServer, IDisposable
{
    private readonly DockerClient _client = new DockerClientConfiguration(endpoint).CreateClient();

    /// <summary>
    /// Gets the host of the vision server.
    /// </summary>
    public string Host => endpoint.Host;

    /// <summary>
    /// Gets the vision services available on the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of vision services.</returns>
    public Task<IEnumerable<IVisionService>> Services => ListContainers();

    /// <summary>
    /// Lists the containers running on the Docker server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of vision services.</returns>
    private async Task<IEnumerable<IVisionService>> ListContainers()
    {
        var parameter = new ContainersListParameters { All = true };
        var containers = await _client.Containers.ListContainersAsync(parameter);

        return containers.Select(it => new VisionServiceInDocker(it, _client));
    }

    /// <summary>
    /// Disposes the resources used by the VisionServerInDocker.
    /// </summary>
    public void Dispose() => _client.Dispose();
}
