namespace Abstractions.VisionServices;

public interface IVisionService : IDisposable
{
    string Id { get; }

    string Name { get; }

    string Description { get; }

    ServiceStateTracer State { get; }

    Task Start();

    Task Stop();

    Task Restart();
}