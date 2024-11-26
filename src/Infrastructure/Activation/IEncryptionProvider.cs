namespace Infrastructure.Activation;

/// <summary>
/// Defines the contract for an encryption provider.
/// </summary>
public interface IEncryptionProvider
{
    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text.</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts the specified cipher text.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <returns>The decrypted plain text.</returns>
    string Decrypt(string cipherText);
}
