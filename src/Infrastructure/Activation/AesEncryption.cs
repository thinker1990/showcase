using System.Security.Cryptography;

namespace Infrastructure.Activation;

/// <summary>
/// Provides AES encryption and decryption.
/// </summary>
public sealed class AesEncryption : IEncryptionProvider
{
    private static readonly byte[] Key = "BsTeuZMEBLriiJXMzvMu2wu3ek78bt0DJ5Uhto4sZj4=".ToBytes();
    private static readonly byte[] Vector = "O1Bk6Mwr7O713b/j5v/rXA==".ToBytes();

    /// <summary>
    /// Encrypts the specified plain text using AES encryption.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text as a base64 string.</returns>
    /// <exception cref="ArgumentException">Thrown when the plain text is null or empty.</exception>
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

    /// <summary>
    /// Decrypts the specified cipher text using AES decryption.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <returns>The decrypted plain text.</returns>
    /// <exception cref="ArgumentException">Thrown when the cipher text is null or empty.</exception>
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

    /// <summary>
    /// Converts the specified memory stream to a base64 string.
    /// </summary>
    /// <param name="memory">The memory stream to convert.</param>
    /// <returns>The base64 string representation of the memory stream.</returns>
    private static string ToBase64String(MemoryStream memory) =>
        Convert.ToBase64String(memory.ToArray());

    /// <summary>
    /// Converts the specified base64 string to a memory stream.
    /// </summary>
    /// <param name="input">The base64 string to convert.</param>
    /// <returns>The memory stream representation of the base64 string.</returns>
    private static MemoryStream FromBase64String(string input) =>
        new(input.ToBytes());
}
