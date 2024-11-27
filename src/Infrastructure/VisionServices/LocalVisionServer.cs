using Abstractions.VisionServices;
using System.Collections.Immutable;

namespace Infrastructure.VisionServices;

/// <summary>
/// Represents a local vision server.
/// </summary>
public sealed class LocalVisionServer : IVisionServer
{
    private ImmutableDictionary<string, string> _services;

    private static readonly Lazy<LocalVisionServer> Lazy =
        new(() => new LocalVisionServer());

    /// <summary>
    /// Gets the singleton instance of the LocalVisionServer.
    /// </summary>
    public static LocalVisionServer Instance => Lazy.Value;

    /// <summary>
    /// Gets the host of the vision server.
    /// </summary>
    public string Host => Localhost.ToString();

    /// <summary>
    /// Gets the vision services available on the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of vision services.</returns>
    public Task<IEnumerable<IVisionService>> Services => RefreshServices();

    private LocalVisionServer()
    {
        _services = ImmutableDictionary<string, string>.Empty;
    }

    /// <summary>
    /// Adds a vision service to the server.
    /// </summary>
    /// <param name="name">The name of the vision service.</param>
    /// <param name="description">The description of the vision service.</param>
    public void AddService(string name, string description)
    {
        WindowsService.TryFindService(name);
        _services = _services.Add(name, description);
    }

    /// <summary>
    /// Refreshes the list of vision services available on the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of vision services.</returns>
    private async Task<IEnumerable<IVisionService>> RefreshServices()
    {
        await Task.CompletedTask;
        return _services.Select(pair => new WindowsService(pair.Key, pair.Value));
    }
}
