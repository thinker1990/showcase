using Infrastructure.VisionServices;

namespace Infrastructure.UnitTests.VisionServices;

[Category(LongRunning)]
internal sealed class VisionServiceInDockerTests
{
    private const string RunningState = "Running";
    private const string StoppedState = "Exited";
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private IVisionService _service = default!;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

    [SetUp]
    public async Task Setup()
    {
        // ReSharper disable PossibleMultipleEnumeration
        var services = await new VisionServerInDocker(VisionServerInDockerTests.Endpoint).Services;
        var index = new Random().Next(services.Count());
        _service = services.ElementAt(index);
    }

    [Test]
    public void ShouldRetrieveServiceInformationCorrectly()
    {
        _service.Id.Should().NotBeEmpty();
        _service.Name.Should().NotBeEmpty();
        _service.Description.Should().NotBeEmpty();
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
    public Task TearDown() => _service.Start();

    private string ServiceState => _service.State.Current;
}