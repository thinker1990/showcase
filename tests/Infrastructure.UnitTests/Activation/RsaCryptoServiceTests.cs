namespace Infrastructure.UnitTests.Activation;

internal sealed class RsaCryptoServiceTests
{
    private readonly RsaCryptoService _rsa = new(PublicKey);
    private readonly ActivationFileParser _parser = new(ProofFile("trial.sys"));

    [Test]
    public async Task VerifyShouldPassWhenContentAndSignatureNotTampered()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();

        _rsa.Verify(content, signature).Should().BeTrue();
    }

    [Test]
    public async Task VerifyShouldFailWhenContentIsTampered()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();

        var tamperedContent = content[1..];

        _rsa.Verify(tamperedContent, signature).Should().BeFalse();
    }

    [Test]
    public async Task VerifyShouldFailWhenSignatureIsTampered()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();

        signature[0] += 1;

        _rsa.Verify(content, signature).Should().BeFalse();
    }

    [Test]
    public async Task VerifyShouldFailWhenPublicKeyIsTampered()
    {
        var content = await _parser.Content();
        var signature = await _parser.Signature();

        var tamperedPublicKey = PublicKey.Replace("0", "O");

        new RsaCryptoService(tamperedPublicKey).Verify(content, signature).Should().BeFalse();
    }
}