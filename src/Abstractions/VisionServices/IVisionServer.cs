namespace Abstractions.VisionServices;

/// <summary>
/// Defines the contract for a vision server.
/// </summary>
public interface IVisionServer
{
    /// <summary>
    /// Gets the host of the vision server.
    /// </summary>
    string Host { get; }

    /// <summary>
    /// Gets the vision services available on the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of vision services.</returns>
    Task<IEnumerable<IVisionService>> Services { get; }
}
