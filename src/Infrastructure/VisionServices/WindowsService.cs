using Abstractions.Extensions;
using Abstractions.VisionServices;
using System.Security.Principal;
using System.ServiceProcess;
using static System.ServiceProcess.ServiceController;
using static System.ServiceProcess.ServiceControllerStatus;

namespace Infrastructure.VisionServices;

/// <summary>
/// Represents a Windows service as a vision service.
/// </summary>
internal sealed class WindowsService : IVisionService
{
    private readonly ServiceController _service;
    private readonly TimeSpan _timeout = 5.Seconds();

    /// <summary>
    /// Gets the unique identifier of the vision service.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name of the vision service.
    /// </summary>
    public string Name => _service.ServiceName;

    /// <summary>
    /// Gets the description of the vision service.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the state tracer of the vision service.
    /// </summary>
    public ServiceStateTracer State { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsService"/> class.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="description">The description of the service.</param>
    internal WindowsService(string name, string description)
    {
        _service = TryFindService(name);

        Id = UniqueId();
        Description = description;
        State = new ServiceStateTracer(_service.Status.ToString());
    }

    /// <summary>
    /// Tries to find a service by its name.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <returns>The service controller.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the service is not found.</exception>
    internal static ServiceController TryFindService(string name) =>
        GetServices().FirstOrDefault(it => it.ServiceName == name)
        ?? throw new EntityNotFoundException(name);

    /// <summary>
    /// Starts the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Start() => Operate(service => service.Start(), Running);

    /// <summary>
    /// Stops the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Stop() => Operate(service => service.Stop(), Stopped);

    /// <summary>
    /// Restarts the vision service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Restart()
    {
        await Stop();
        await Start();
        UpdateState();
    }

    /// <summary>
    /// Operates the vision service.
    /// </summary>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="targetStatus">The target status of the service.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Updates the state of the vision service.
    /// </summary>
    private void UpdateState()
    {
        _service.Refresh();
        State.Update(_service.Status.ToString());
    }

    /// <summary>
    /// Ensures that the current user has permission to operate the service.
    /// </summary>
    /// <exception cref="RecoverableException">Thrown when the user does not have permission.</exception>
    private static void EnsureHasPermission()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var isAdmin = new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        if (!isAdmin)
        {
            throw new RecoverableException("Permission denied. Please run as Administrator.");
        }
    }

    /// <summary>
    /// Disposes the resources used by the vision service.
    /// </summary>
    public void Dispose()
    {
        State.Dispose();
        _service.Dispose();
    }
}
