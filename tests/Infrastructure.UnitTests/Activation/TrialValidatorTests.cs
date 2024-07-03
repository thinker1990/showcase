namespace Infrastructure.UnitTests.Activation;

[Category(ExternalDependency)]
internal sealed class TrialValidatorTests
{
    [Test]
    public void ShouldThrowExceptionWhenInitializeWithNotExistFile()
    {
        var validator = new TrialValidator(PublicKey, new FileInfo("FileNotExist.sys"));

        var verification = validator.Validate;

        verification.Should().ThrowAsync<FileNotFoundException>();
    }

    [Test]
    public async Task VerificationShouldPassWhenGivenValidTrialFile()
    {
        var validator = new TrialValidator(PublicKey, Valid);

        var verification = validator.Validate;

        await verification.Should().NotThrowAsync();
    }

    [Test]
    public async Task VerificationShouldFailWhenGivenExpiredTrialFile()
    {
        var validator = new TrialValidator(PublicKey, Expired);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<ExpiredException>();
    }

    [Test]
    public async Task VerificationShouldFailWhenGivenIncorrectFormatTrialFile()
    {
        var validator = new TrialValidator(PublicKey, IncorrectFormat);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<IncorrectFileFormatException>();
    }

    [Test]
    public async Task VerificationShouldFailWhenGivenIncorrectTypeTrialFile()
    {
        var validator = new TrialValidator(PublicKey, IncorrectType);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<IncorrectFileFormatException>();
    }

    private static FileInfo Valid => ProofFile("trial.sys");

    private static FileInfo Expired => ProofFile("trial.expired.sys");

    private static FileInfo IncorrectFormat => ProofFile("trial.incorrect.format.sys");

    private static FileInfo IncorrectType => ProofFile("trial.incorrect.file.dll");
}