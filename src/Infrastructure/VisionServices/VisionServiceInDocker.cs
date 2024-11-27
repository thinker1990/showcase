using Abstractions.VisionServices;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Infrastructure.VisionServices;

/// <summary>  
/// Represents a vision service running in a Docker container.  
/// </summary>  
internal sealed class VisionServiceInDocker : IVisionService
{
    private const char NameSeparator = '/';
    private readonly DockerClient _client;

    /// <summary>  
    /// Gets the unique identifier of the vision service.  
    /// </summary>  
    public string Id { get; }

    /// <summary>  
    /// Gets the name of the vision service.  
    /// </summary>  
    public string Name { get; }

    /// <summary>  
    /// Gets the description of the vision service.  
    /// </summary>  
    public string Description { get; }

    /// <summary>  
    /// Gets the state tracer of the vision service.  
    /// </summary>  
    public ServiceStateTracer State { get; }

    /// <summary>  
    /// Initializes a new instance of the <see cref="VisionServiceInDocker"/> class.  
    /// </summary>  
    /// <param name="container">The container list response.</param>  
    /// <param name="client">The Docker client.</param>  
    internal VisionServiceInDocker(ContainerListResponse container, DockerClient client)
    {
        _client = client;
        Id = container.ID;
        Name = NameOf(container);
        Description = ImageOf(container);
        State = new ServiceStateTracer(container.State);
    }

    /// <summary>  
    /// Starts the vision service.  
    /// </summary>  
    /// <returns>A task that represents the asynchronous operation.</returns>  
    public async Task Start()
    {
        await _client.Containers.StartContainerAsync(Id, new ContainerStartParameters());
        await UpdateState();
    }

    /// <summary>  
    /// Stops the vision service.  
    /// </summary>  
    /// <returns>A task that represents the asynchronous operation.</returns>  
    public async Task Stop()
    {
        await _client.Containers.StopContainerAsync(Id, new ContainerStopParameters());
        await UpdateState();
    }

    /// <summary>  
    /// Restarts the vision service.  
    /// </summary>  
    /// <returns>A task that represents the asynchronous operation.</returns>  
    public async Task Restart()
    {
        await _client.Containers.RestartContainerAsync(Id, new ContainerRestartParameters());
        await UpdateState();
    }

    /// <summary>  
    /// Gets the name of the container.  
    /// </summary>  
    /// <param name="container">The container list response.</param>  
    /// <returns>The name of the container.</returns>  
    private static string NameOf(ContainerListResponse container) =>
        container.Names.First().TrimStart(NameSeparator);

    /// <summary>  
    /// Updates the state of the vision service.  
    /// </summary>  
    /// <returns>A task that represents the asynchronous operation.</returns>  
    private async Task UpdateState()
    {
        var detail = await _client.Containers.InspectContainerAsync(Id);
        State.Update(detail.State.Status);
    }

    /// <summary>  
    /// Gets the image of the container.  
    /// </summary>  
    /// <param name="container">The container list response.</param>  
    /// <returns>The image of the container.</returns>  
    private static string ImageOf(ContainerListResponse container) =>
        container.Image.Split(NameSeparator).Last();

    /// <summary>  
    /// Disposes the resources used by the vision service.  
    /// </summary>  
    public void Dispose() => State.Dispose();
}
