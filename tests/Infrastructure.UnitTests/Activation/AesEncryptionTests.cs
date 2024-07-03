namespace Infrastructure.UnitTests.Activation;

internal sealed class AesEncryptionTests
{
    private const string PlainText = "Here is some data to encrypt!";
    private readonly IEncryptionProvider _aes = new AesEncryption();

    [Test]
    public void ShouldEncryptPlainTextToCipherText()
    {
        var cipherText = _aes.Encrypt(PlainText);

        cipherText.Should().NotBe(PlainText);
    }

    [Test]
    public void ShouldDecryptCipherTextToPlainText()
    {
        var cipherText = _aes.Encrypt(PlainText);

        var plainText = _aes.Decrypt(cipherText);

        plainText.Should().Be(PlainText);
    }

    [Test]
    public void CipherTextShouldBeSameWhenEncryptSamePlainTextMultipleTimes()
    {
        var cipherText1 = _aes.Encrypt(PlainText);

        var cipherText2 = _aes.Encrypt(PlainText);

        cipherText1.Should().Be(cipherText2);
    }

    [Test]
    public void ShouldThrowExceptionWhenPlainTextIsNullOrEmpty()
    {
        var encryptNull = () => _aes.Encrypt(null!);
        var encryptEmpty = () => _aes.Encrypt(string.Empty);

        encryptNull.Should().Throw<ArgumentNullException>();
        encryptEmpty.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ShouldThrowExceptionWhenCipherTextIsNullOrEmpty()
    {
        var decryptNull = () => _aes.Decrypt(null!);
        var decryptEmpty = () => _aes.Decrypt(string.Empty);

        decryptNull.Should().Throw<ArgumentNullException>();
        decryptEmpty.Should().Throw<ArgumentException>();
    }
}