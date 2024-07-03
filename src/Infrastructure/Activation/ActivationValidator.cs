namespace Infrastructure.Activation;

public sealed class ActivationValidator(string publicKey, FileInfo activationFile)
{
    private readonly ActivationFileParser _parser = new(activationFile);
    private readonly RsaCryptoService _rsa = new(publicKey);

    public async Task<ValidityPeriod> Validate()
    {
        await VerifySignature();
        await VerifyMachineFingerprint();
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

    private async Task VerifyMachineFingerprint()
    {
        var actual = await _parser.MachineFingerprint();
        var expected = HardwareInformation.Fingerprint();
        if (expected != actual)
        {
            throw new FingerprintMismatchException();
        }
    }

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