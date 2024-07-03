using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Activation;

internal sealed class RsaCryptoService
{
    private readonly RSACryptoServiceProvider _rsaCrypto;

    public RsaCryptoService(string publicKey)
    {
        _rsaCrypto = new RSACryptoServiceProvider(512);
        _rsaCrypto.FromXmlString(RsaKeyValue(publicKey));
    }

    public bool Verify(string content, byte[] signature)
    {
        using var sha512 = SHA512.Create();
        var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(content));

        return _rsaCrypto.VerifyHash(hash, Sha512Oid, signature);
    }

    private static string RsaKeyValue(string publicKey) =>
        $"""
         <RSAKeyValue>
            <Modulus>{publicKey}</Modulus>
            <Exponent>AQAB</Exponent>
         </RSAKeyValue>
         """;

    private static string Sha512Oid => CryptoConfig.MapNameToOID(nameof(SHA512))!;
}