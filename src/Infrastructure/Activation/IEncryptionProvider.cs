namespace Infrastructure.Activation;

public interface IEncryptionProvider
{
    string Encrypt(string plainText);

    string Decrypt(string cipherText);
}