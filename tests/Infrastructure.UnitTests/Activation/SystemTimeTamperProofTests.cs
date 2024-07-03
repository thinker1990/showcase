using Abstractions.Extensions;
using System.Globalization;
using System.Reactive.Linq;

namespace Infrastructure.UnitTests.Activation;

[Category(ExternalDependency)]
internal sealed class SystemTimeTamperProofTests
{
    private FileInfo _timeRecorder = default!;
    private SystemTimeTamperProof _timeTamperProof = default!;

    [SetUp]
    public async Task Setup()
    {
        _timeRecorder = RandomTemporaryFile();
        _timeTamperProof = new SystemTimeTamperProof(_timeRecorder, 50.Milliseconds(), NoEncryption());
        await _timeTamperProof.Bootstrap();
    }

    [Test]
    public async Task TimeRecordFileShouldBeCreatedAfterBootstrap()
    {
        var proof = new SystemTimeTamperProof(_timeRecorder, 1.Seconds(), NoEncryption());

        var result = await proof.Bootstrap();

        _timeRecorder.Exists().Should().BeTrue();
        result.Should().Be(SystemTimeValidateResult.Normal);
    }

    [Test]
    public async Task ValidateResultShouldBeNormalWhenNotTamperSystemTime()
    {
        var result = await _timeTamperProof.ValidationStream.FirstAsync();

        result.Should().Be(SystemTimeValidateResult.Normal);
    }

    [Test]
    public async Task ValidateResultShouldBeRecordLostWhenTimeRecordFileNotExists()
    {
        _timeRecorder.Delete();

        var result = await _timeTamperProof.ValidationStream.FirstAsync();

        result.Should().Be(SystemTimeValidateResult.RecordLost);
    }

    [Test]
    public async Task ValidateResultShouldBeInvalidWhenTimeRecordIsTampered()
    {
        await "nonsense".WriteTo(_timeRecorder);

        var result = await _timeTamperProof.ValidationStream.FirstAsync();

        result.Should().Be(SystemTimeValidateResult.Invalid);
    }

    [Test]
    public async Task ValidateResultShouldBeHijackedWhenLatestTimeIsLateThanNow()
    {
        await OneHourLater.WriteTo(_timeRecorder);

        var result = await _timeTamperProof.ValidationStream.FirstAsync();

        result.Should().Be(SystemTimeValidateResult.Hijacked);
    }

    [Test]
    public async Task ValidateResultShouldBeHijackedWhenLatestTimeIsLateThanNowWithNewProof()
    {
        await OneHourLater.WriteTo(_timeRecorder);
        var newProof = new SystemTimeTamperProof(_timeRecorder, 50.Milliseconds(), NoEncryption());

        var result = await newProof.ValidationStream.FirstAsync();

        result.Should().Be(SystemTimeValidateResult.Hijacked);
    }

    [Test]
    public async Task TimeRecordShouldBeUpdatedAfterValidationPassed()
    {
        await OneHourAgo.WriteTo(_timeRecorder);
        var before = await _timeRecorder.AllText();

        await _timeTamperProof.ValidationStream.Take(2);

        var after = await _timeRecorder.AllText();
        after.Should().NotBe(before);
    }

    [TearDown]
    public async Task Teardown()
    {
        _timeTamperProof.Dispose();
        try
        {
            _timeRecorder.Delete();
        }
        catch (IOException)
        {
            await Task.Delay(500.Milliseconds());
            _timeRecorder.Delete();
        }
    }

    private static string OneHourLater =>
        DateTime.UtcNow.AddHours(1).ToString(CultureInfo.InvariantCulture);

    private static string OneHourAgo =>
        DateTime.UtcNow.AddHours(-1).ToString(CultureInfo.InvariantCulture);
}