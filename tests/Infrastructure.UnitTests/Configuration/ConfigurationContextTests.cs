using Infrastructure.Configuration;

namespace Infrastructure.UnitTests.Configuration;

[Category(ExternalDependency)]
internal sealed class ConfigurationContextTests
{
    private readonly IDbContextFactory<ConfigurationContext> _factory = ConfigurationContextFactory();
    private ConfigurationContext _context = default!;

    [SetUp]
    public async Task Setup()
    {
        _context = await _factory.CreateDbContextAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    [Test]
    public void DataSetsShouldNotBeNull()
    {
        _context.Settings.Should().NotBeNull();
    }

    [TestCase("SomeKey", "SomeValue")]
    public async Task ShouldAddSettingsCorrectly(string key, string value)
    {
        var settings = new Settings(key, value);

        _context.Settings.Add(settings);
        await _context.SaveChangesAsync();

        _context.Settings.Should().Contain(it => it.Key == key);
    }

    [Test]
    public void ShouldThrowExceptionWhenAddSettingsWithTheSameKey()
    {
        var (key, value1, value2) = ("key", "value1", "value2");

        _context.Settings.Add(new Settings(key, value1));
        var tryAdd = () => _context.Settings.Add(new Settings(key, value2));

        tryAdd.Should().Throw<InvalidOperationException>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }
}