namespace Infrastructure.Activation;

/// <summary>
/// Validates the trial file using the provided public key.
/// </summary>
/// <param name="publicKey">The public key used for signature verification.</param>
/// <param name="trialFile">The trial file to validate.</param>
public sealed class TrialValidator(string publicKey, FileInfo trialFile)
{
    private readonly ActivationFileParser _parser = new(trialFile);
    private readonly RsaCryptoService _rsa = new(publicKey);

    /// <summary>
    /// Validates the trial file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validity period of the trial.</returns>
    /// <exception cref="SignatureVerificationException">Thrown when the signature verification fails.</exception>
    /// <exception cref="ExpiredException">Thrown when the trial validity period has expired.</exception>
    public async Task<ValidityPeriod> Validate()
    {
        await VerifySignature();
        return await VerifyValidityPeriod();
    }

    /// <summary>
    /// Verifies the signature of the trial file.
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
    /// Verifies the validity period of the trial file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validity period of the trial.</returns>
    /// <exception cref="ExpiredException">Thrown when the trial validity period has expired.</exception>
    private async Task<ValidityPeriod> VerifyValidityPeriod()
    {
        var validityPeriod = await _parser.TrialValidityPeriod();
        if (validityPeriod.Expired())
        {
            throw new ExpiredException();
        }

        return validityPeriod;
    }
}
