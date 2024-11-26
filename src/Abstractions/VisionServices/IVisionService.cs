namespace Abstractions.VisionServices;

/// <summary>
/// Defines the contract for a vision service.
/// </summary>
public interface IVisionService : IDisposable
{
    /// <summary>
    /// Gets the unique identifier of the vision service.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the vision service.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the vision service.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the state tracer of the vision service.
    /// </summary>
    ServiceStateTracer State { get; }

    /// <summary>
    /// Starts the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Start();

    /// <summary>
    /// Stops the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Stop();

    /// <summary>
    /// Restarts the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Restart();
}
