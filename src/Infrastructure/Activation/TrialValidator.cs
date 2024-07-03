namespace Infrastructure.Activation;

public sealed class TrialValidator(string publicKey, FileInfo trialFile)
{
    private readonly ActivationFileParser _parser = new(trialFile);
    private readonly RsaCryptoService _rsa = new(publicKey);

    public async Task<ValidityPeriod> Validate()
    {
        await VerifySignature();
        return await VerifyValidityPeriod();
    }

    private async Task VerifySignature()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();
        if (!_rsa.Verify(content, signature))
        {
            throw new SignatureVerificationException();
        }
    }

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