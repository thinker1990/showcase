using Infrastructure.VisionServices;

namespace Infrastructure.UnitTests.VisionServices;

[SkipIfNotAdministrator]
[Category(ExternalDependency)]
internal sealed class WindowsServiceTests
{
    private const string Name = "DefectDetectionService";
    private const string Description = "gRPC service";
    private const string RunningState = "Running";
    private const string StoppedState = "Stopped";
    private WindowsService _service = default!;

    [SetUp]
    public void Setup() => _service = new WindowsService(Name, Description);

    [Test]
    public void ShouldRetrieveServiceInformationCorrectly()
    {
        _service.Id.Should().NotBeEmpty();
        _service.Name.Should().Be(Name);
        _service.Description.Should().Be(Description);
    }

    [Test]
    public async Task ShouldBeRunningAfterStartService()
    {
        await _service.Stop();
        ServiceState.Should().Be(StoppedState);

        await _service.Start();

        ServiceState.Should().Be(RunningState);
    }

    [Test]
    public async Task ShouldBeStoppedAfterStopService()
    {
        await _service.Start();
        ServiceState.Should().Be(RunningState);

        await _service.Stop();

        ServiceState.Should().Be(StoppedState);
    }

    [Test]
    public async Task ShouldBeRunningAfterRestartARunningService()
    {
        await _service.Start();
        ServiceState.Should().Be(RunningState);

        await _service.Restart();

        ServiceState.Should().Be(RunningState);
    }

    [Test]
    public async Task ShouldBeRunningAfterRestartAStoppedService()
    {
        await _service.Stop();
        ServiceState.Should().Be(StoppedState);

        await _service.Restart();

        ServiceState.Should().Be(RunningState);
    }

    [Test]
    public async Task StartServiceShouldBeIdempotent()
    {
        await _service.Start();
        ServiceState.Should().Be(RunningState);

        await _service.Start();

        ServiceState.Should().Be(RunningState);
    }

    [Test]
    public async Task StopServiceShouldBeIdempotent()
    {
        await _service.Stop();
        ServiceState.Should().Be(StoppedState);

        await _service.Stop();

        ServiceState.Should().Be(StoppedState);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _service.Start();
        _service.Dispose();
    }

    private string ServiceState => _service.State.Current;
}