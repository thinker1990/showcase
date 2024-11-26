using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authorization;

/// <summary>
/// Provides methods for hashing passwords.
/// </summary>
public static class PasswordHasher
{
    private const int KeySize = 64;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /// <summary>
    /// Hashes the specified password using the specified salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use for hashing.</param>
    /// <returns>The hashed password as a base64 string.</returns>
    /// <exception cref="ArgumentException">Thrown when the password or salt is null or empty.</exception>
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
