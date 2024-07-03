using System.Security.Cryptography;

namespace Infrastructure.Activation;

public sealed class AesEncryption : IEncryptionProvider
{
    private static readonly byte[] Key = "BsTeuZMEBLriiJXMzvMu2wu3ek78bt0DJ5Uhto4sZj4=".ToBytes();
    private static readonly byte[] Vector = "O1Bk6Mwr7O713b/j5v/rXA==".ToBytes();

    public string Encrypt(string plainText)
    {
        EnsureNotEmpty(plainText, nameof(plainText));

        using var aes = Aes.Create();
        var encryptor = aes.CreateEncryptor(Key, Vector);
        using var memory = new MemoryStream();
        using var encrypt = new CryptoStream(memory, encryptor, CryptoStreamMode.Write);
        using (var writer = new StreamWriter(encrypt))
        {
            writer.Write(plainText);
        }

        return ToBase64String(memory);
    }

    public string Decrypt(string cipherText)
    {
        EnsureNotEmpty(cipherText, nameof(cipherText));

        using var aes = Aes.Create();
        var decryptor = aes.CreateDecryptor(Key, Vector);
        using var memory = FromBase64String(cipherText);
        using var decrypt = new CryptoStream(memory, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(decrypt);

        return reader.ReadToEnd();
    }

    private static string ToBase64String(MemoryStream memory) =>
        Convert.ToBase64String(memory.ToArray());

    private static MemoryStream FromBase64String(string input) =>
        new(input.ToBytes());
}