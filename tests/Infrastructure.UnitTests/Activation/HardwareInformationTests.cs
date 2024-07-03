using static Infrastructure.Activation.HardwareInformation;

namespace Infrastructure.UnitTests.Activation;

internal sealed class HardwareInformationTests
{
    [Test]
    public void ProcessorIdShouldNotBeNullOrEmpty()
    {
        ProcessorId().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void BiosIdShouldNotBeNullOrEmpty()
    {
        BiosId().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void DiskIdShouldNotBeNullOrEmpty()
    {
        DiskId().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void VolumeSerialNumberShouldNotBeNullOrEmpty()
    {
        VolumeSerialNumber().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void BaseBoardIdShouldNotBeNullOrEmpty()
    {
        BaseBoardId().Should().NotBeNullOrEmpty();
    }

    [Ignore("Specific machine.")]
    [TestCase("QkZFQkZCRkYwMDA4MDZDMVBGMlJWMUc0U0tIeW5peF9IRk01MTJHRDNIWDAxNU5BQTAwQ0E0RkwxSEYxNzkxNjUz")]
    public void FingerprintShouldBeSameAsPreCollected(string expected)
    {
        Fingerprint().Should().Be(expected);
    }
}