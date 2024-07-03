using Abstractions.VisionServices;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Infrastructure.VisionServices;

internal sealed class VisionServiceInDocker : IVisionService
{
    private const char NameSeparator = '/';

    private readonly DockerClient _client;

    public string Id { get; }

    public string Name { get; }

    public string Description { get; }

    public ServiceStateTracer State { get; }

    internal VisionServiceInDocker(ContainerListResponse container, DockerClient client)
    {
        _client = client;
        Id = container.ID;
        Name = NameOf(container);
        Description = ImageOf(container);
        State = new ServiceStateTracer(container.State);
    }

    public async Task Start()
    {
        await _client.Containers.StartContainerAsync(Id, new ContainerStartParameters());
        await UpdateState();
    }

    public async Task Stop()
    {
        await _client.Containers.StopContainerAsync(Id, new ContainerStopParameters());
        await UpdateState();
    }

    public async Task Restart()
    {
        await _client.Containers.RestartContainerAsync(Id, new ContainerRestartParameters());
        await UpdateState();
    }

    private static string NameOf(ContainerListResponse container) =>
        container.Names.First().TrimStart(NameSeparator);

    private async Task UpdateState()
    {
        var detail = await _client.Containers.InspectContainerAsync(Id);
        State.Update(detail.State.Status);
    }

    private static string ImageOf(ContainerListResponse container) =>
        container.Image.Split(NameSeparator).Last();

    public void Dispose() => State.Dispose();
}