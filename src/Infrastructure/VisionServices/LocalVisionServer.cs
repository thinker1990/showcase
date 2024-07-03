using Abstractions.VisionServices;
using System.Collections.Immutable;

namespace Infrastructure.VisionServices;

public sealed class LocalVisionServer : IVisionServer
{
    private ImmutableDictionary<string, string> _services;

    private static readonly Lazy<LocalVisionServer> Lazy =
        new(() => new LocalVisionServer());

    public static LocalVisionServer Instance => Lazy.Value;

    public string Host => Localhost.ToString();

    public Task<IEnumerable<IVisionService>> Services => RefreshServices();

    private LocalVisionServer()
    {
        _services = ImmutableDictionary<string, string>.Empty;
    }

    public void AddService(string name, string description)
    {
        WindowsService.TryFindService(name);
        _services = _services.Add(name, description);
    }

    private async Task<IEnumerable<IVisionService>> RefreshServices()
    {
        await Task.CompletedTask;
        return _services.Select(pair => new WindowsService(pair.Key, pair.Value));
    }
}