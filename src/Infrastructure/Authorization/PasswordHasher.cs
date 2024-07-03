using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authorization;

public static class PasswordHasher
{
    private const int KeySize = 64;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public static string Hash(string password, string salt)
    {
        EnsureNotEmpty(password, nameof(password));
        EnsureNotEmpty(salt, nameof(salt));

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            Encoding.UTF8.GetBytes(salt),
            Iterations, HashAlgorithm, KeySize);

        return Convert.ToBase64String(hash);
    }
}