namespace Infrastructure.UnitTests.Activation;

[Category(ExternalDependency)]
internal sealed class ActivationValidatorTests
{
    [Test]
    public void ShouldThrowExceptionWhenInitializeWithNotExistFile()
    {
        var validator = new ActivationValidator(PublicKey, new FileInfo("FileNotExist.sys"));

        var verification = validator.Validate;

        verification.Should().ThrowAsync<FileNotFoundException>();
    }

    [Test]
    public async Task VerificationShouldPassWhenGivenValidActivationFile()
    {
        var validator = new ActivationValidator(PublicKey, Valid);

        var verification = validator.Validate;

        await verification.Should().NotThrowAsync();
    }

    [Test]
    public async Task VerificationShouldBeIdempotentWhenActivateMultipleTimes()
    {
        var validator = new ActivationValidator(PublicKey, Valid);

        await validator.Validate();
        var validateAgain = validator.Validate;

        await validateAgain.Should().NotThrowAsync();
    }

    [Test]
    public async Task VerificationShouldFailWhenGivenExpiredActivationFile()
    {
        var validator = new ActivationValidator(PublicKey, Expired);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<ExpiredException>();
    }

    [Test]
    public async Task VerificationShouldFailWhenFingerprintMismatch()
    {
        var validator = new ActivationValidator(PublicKey, FingerprintMismatch);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<FingerprintMismatchException>();
    }

    [Test]
    public async Task VerificationShouldFailWhenSignatureMismatch()
    {
        var validator = new ActivationValidator(PublicKey, SignatureMismatch);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<SignatureVerificationException>();
    }

    [Test]
    public async Task VerificationShouldFailWhenGivenTrialFile()
    {
        var validator = new ActivationValidator(PublicKey, TrialFile);

        var verification = validator.Validate;

        await verification.Should().ThrowAsync<IncorrectFileFormatException>();
    }

    private static FileInfo Valid => ProofFile("activation.sys");

    private static FileInfo Expired => ProofFile("activation.expired.sys");

    private static FileInfo FingerprintMismatch => ProofFile("activation.fingerprint.mismatch.sys");

    private static FileInfo SignatureMismatch => ProofFile("activation.signature.mismatch.sys");

    private static FileInfo TrialFile => ProofFile("trial.sys");
}