using Abstractions.Extensions;
using Abstractions.VisionServices;
using System.Security.Principal;
using System.ServiceProcess;
using static System.ServiceProcess.ServiceController;
using static System.ServiceProcess.ServiceControllerStatus;

namespace Infrastructure.VisionServices;

internal sealed class WindowsService : IVisionService
{
    private readonly ServiceController _service;
    private readonly TimeSpan _timeout = 5.Seconds();

    public string Id { get; }

    public string Name => _service.ServiceName;

    public string Description { get; }

    public ServiceStateTracer State { get; }

    internal WindowsService(string name, string description)
    {
        _service = TryFindService(name);

        Id = UniqueId();
        Description = description;
        State = new ServiceStateTracer(_service.Status.ToString());
    }

    internal static ServiceController TryFindService(string name) =>
        GetServices().FirstOrDefault(it => it.ServiceName == name)
        ?? throw new EntityNotFoundException(name);

    public Task Start() => Operate(service => service.Start(), Running);

    public Task Stop() => Operate(service => service.Stop(), Stopped);

    public async Task Restart()
    {
        await Stop();
        await Start();
        UpdateState();
    }

    private Task Operate(Action<ServiceController> operation, ServiceControllerStatus targetStatus)
    {
        if (_service.Status == targetStatus) return Task.CompletedTask;

        EnsureHasPermission();
        return Task.Run(() =>
        {
            operation(_service);
            _service.WaitForStatus(targetStatus, _timeout);
            UpdateState();
        });
    }

    private void UpdateState()
    {
        _service.Refresh();
        State.Update(_service.Status.ToString());
    }

    private static void EnsureHasPermission()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var isAdmin = new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        if (!isAdmin)
        {
            throw new RecoverableException("Permission denied. Please run as Administrator.");
        }
    }

    public void Dispose()
    {
        State.Dispose();
        _service.Dispose();
    }
}