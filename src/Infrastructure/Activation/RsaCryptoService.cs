using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Activation;

/// <summary>
/// Provides RSA cryptographic services for verifying signatures.
/// </summary>
internal sealed class RsaCryptoService
{
    private readonly RSACryptoServiceProvider _rsaCrypto;

    /// <summary>
    /// Initializes a new instance of the <see cref="RsaCryptoService"/> class with the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key used for signature verification.</param>
    public RsaCryptoService(string publicKey)
    {
        _rsaCrypto = new RSACryptoServiceProvider(512);
        _rsaCrypto.FromXmlString(RsaKeyValue(publicKey));
    }

    /// <summary>
    /// Verifies the signature of the specified content.
    /// </summary>
    /// <param name="content">The content to verify.</param>
    /// <param name="signature">The signature to verify against.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public bool Verify(string content, byte[] signature)
    {
        var hash = SHA512.HashData(Encoding.UTF8.GetBytes(content));
        return _rsaCrypto.VerifyHash(hash, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// Converts the public key to an RSA key value XML string.
    /// </summary>
    /// <param name="publicKey">The public key to convert.</param>
    /// <returns>An RSA key value XML string.</returns>
    private static string RsaKeyValue(string publicKey) =>
        $"""
         <RSAKeyValue>
            <Modulus>{publicKey}</Modulus>
            <Exponent>AQAB</Exponent>
         </RSAKeyValue>
         """;
}
