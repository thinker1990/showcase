namespace Infrastructure.Activation;

/// <summary>
/// Validates the activation file using the provided public key.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ActivationValidator"/> class.
/// </remarks>
/// <param name="publicKey">The public key used for signature verification.</param>
/// <param name="activationFile">The activation file to validate.</param>
public sealed class ActivationValidator(string publicKey, FileInfo activationFile)
{
    private readonly ActivationFileParser _parser = new(activationFile);
    private readonly RsaCryptoService _rsa = new(publicKey);

    /// <summary>
    /// Validates the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validity period of the activation.</returns>
    /// <exception cref="SignatureVerificationException">Thrown when the signature verification fails.</exception>
    /// <exception cref="FingerprintMismatchException">Thrown when the machine fingerprint does not match.</exception>
    /// <exception cref="ExpiredException">Thrown when the activation validity period has expired.</exception>
    public async Task<ValidityPeriod> Validate()
    {
        await VerifySignature();
        await VerifyMachineFingerprint();
        return await VerifyValidityPeriod();
    }

    /// <summary>
    /// Verifies the signature of the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="SignatureVerificationException">Thrown when the signature verification fails.</exception>
    private async Task VerifySignature()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();
        if (!_rsa.Verify(content, signature))
        {
            throw new SignatureVerificationException();
        }
    }

    /// <summary>
    /// Verifies the machine fingerprint in the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="FingerprintMismatchException">Thrown when the machine fingerprint does not match.</exception>
    private async Task VerifyMachineFingerprint()
    {
        var actual = await _parser.MachineFingerprint();
        var expected = HardwareInformation.Fingerprint();
        if (expected != actual)
        {
            throw new FingerprintMismatchException();
        }
    }

    /// <summary>
    /// Verifies the validity period of the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validity period of the activation.</returns>
    /// <exception cref="ExpiredException">Thrown when the activation validity period has expired.</exception>
    private async Task<ValidityPeriod> VerifyValidityPeriod()
    {
        var validityPeriod = await _parser.ActivationValidityPeriod();
        if (validityPeriod.Expired())
        {
            throw new ExpiredException();
        }

        return validityPeriod;
    }
}
