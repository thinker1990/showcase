using Infrastructure.VisionServices;

namespace Infrastructure.UnitTests.VisionServices;

[Category(ExternalDependency)]
internal sealed class VisionServerInDockerTests
{
    private VisionServerInDocker _server = default!;

    internal static Uri Endpoint => EndpointOf("192.168.0.98");

    [SetUp]
    public void Setup() => _server = new VisionServerInDocker(Endpoint);

    [Test]
    public void ShouldRetrieveHostCorrectly()
    {
        _server.Host.Should().Be(Endpoint.Host);
    }

    [Test]
    public async Task ServicesShouldNotBeEmpty()
    {
        var services = await _server.Services;

        services.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldThrowExceptionIfHostHasNoContainers()
    {
        var server = new VisionServerInDocker(EndpointOf(Utility.Localhost.ToString()));

        var fetch = () => server.Services;

        await fetch.Should().ThrowAsync<HttpRequestException>();
    }

    [TearDown]
    public void TearDown() => _server.Dispose();

    private static Uri EndpointOf(string host)
    {
        var builder = new UriBuilder(Uri.UriSchemeHttp, host, 2375);
        return builder.Uri;
    }
}