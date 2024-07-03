using Infrastructure.VisionServices;

namespace Infrastructure.UnitTests.VisionServices;

[Category(ExternalDependency)]
internal sealed class LocalVisionServerTests
{
    private const string AxonService = "Axon";
    private readonly LocalVisionServer _server = LocalVisionServer.Instance;

    [SetUp]
    public void Setup() => _server.AddService(AxonService, string.Empty);

    [Test]
    public void HostShouldBeLocalHost()
    {
        _server.Host.Should().Be(Utility.Localhost.ToString());
    }

    [Test]
    public async Task ServicesShouldContainAxon()
    {
        var services = await _server.Services;

        services.Should().Contain(it => it.Name == AxonService);
    }

    [Test]
    public async Task ShouldAddSucceedWhenAddExistingWindowsService()
    {
        var (name, description) = ("EventLog", "Windows Event Log");

        _server.AddService(name, description);

        var services = await _server.Services;
        services.Should().Contain(it => it.Name == name);
    }

    [Test]
    public void ShouldThrowExceptionWhenTryAddServiceNotExists()
    {
        var (name, description) = ("ServiceNotExists", string.Empty);

        var add = () => _server.AddService(name, description);

        add.Should().Throw<EntityNotFoundException>();
    }
}