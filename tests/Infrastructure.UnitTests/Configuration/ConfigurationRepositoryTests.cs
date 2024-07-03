using Abstractions.Configuration;
using Infrastructure.Configuration;

namespace Infrastructure.UnitTests.Configuration;

[Category(ExternalDependency)]
internal sealed class ConfigurationRepositoryTests
{
    private const string Key = "AnswerToTheUltimateQuestion";
    private const string Value = "42";

    private readonly IDbContextFactory<ConfigurationContext> _factory = ConfigurationContextFactory();
    private IConfigurationManager _repository = default!;

    [SetUp]
    public async Task Setup()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        _repository = new ConfigurationRepository(_factory);
    }

    [TestCase("")]
    [TestCase(" ")]
    public async Task ShouldThrowExceptionWhenAddSettingsWithEmptyKey(string key)
    {
        var create = () => _repository.Add(SettingsOf(key, Value));

        await create.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenAddSettingsWithNullValue()
    {
        var create = () => _repository.Add(SettingsOf(Key, null!));

        await create.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ShouldAddSettingsCorrectlyWhenGivenValidKeyValuePair()
    {
        var succeed = await _repository.Add(SettingsOf(Key, Value));

        succeed.Should().BeTrue();
    }

    [Test]
    public async Task ShouldGetCorrespondingValueOfSpecificKey()
    {
        await _repository.Add(SettingsOf(Key, Value));

        var value = await _repository.Get(Key);

        value.Should().Be(Value);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGetValueOfNotExistKey()
    {
        var tryGet = () => _repository.Get("NotExistKey");

        await tryGet.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenAddDuplicateSettings()
    {
        await _repository.Add(SettingsOf(Key, Value));

        var create = () => _repository.Add(SettingsOf(Key, Value));

        await create.Should().ThrowAsync<EntityDuplicateException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenUpdateSettingsWithWrongKey()
    {
        await _repository.Add(SettingsOf(Key, Value));

        const string wrongKey = $"Wrong{Key}";
        var update = () => _repository.Update(SettingsOf(wrongKey, Value));

        await update.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateCorrespondingValueWhenGivenRightKey()
    {
        await _repository.Add(SettingsOf(Key, Value));
        const string newValue = "forty-two";

        await _repository.Update(SettingsOf(Key, newValue));

        await using var context = CreateContext();
        context.Settings.Should().Contain(it => it.Key == Key && it.Value == newValue);
    }

    [Test]
    public async Task ShouldDeleteCorrespondingSettingsOfSpecificKey()
    {
        await _repository.Add(SettingsOf(Key, Value));
        (await AllSettings()).Should().NotBeEmpty();

        var succeed = await _repository.Delete(Key);

        succeed.Should().BeTrue();
        (await AllSettings()).Should().BeEmpty();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeleteNotExistKey()
    {
        var delete = () => _repository.Delete("NotExistKey");

        await delete.Should().ThrowAsync<EntityNotFoundException>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }

    private ConfigurationContext CreateContext() => _factory.CreateDbContext();

    private Task<IReadOnlyDictionary<string, string>> AllSettings() =>
        ((ConfigurationRepository)_repository).Get();

    private static KeyValuePair<string, string> SettingsOf(string key, string value) =>
        KeyValuePair.Create(key, value);
}