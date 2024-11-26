using System.Text;
using static System.Convert;

namespace Infrastructure.Activation;

/// <summary>
/// Provides extension methods for Base64 encoding and decoding.
/// </summary>
internal static class Base64Extensions
{
    /// <summary>
    /// Converts a Base64 string to a byte array.
    /// </summary>
    /// <param name="base64String">The Base64 string to convert.</param>
    /// <returns>A byte array representation of the Base64 string.</returns>
    internal static byte[] ToBytes(this string base64String) =>
        FromBase64String(base64String);

    /// <summary>
    /// Decodes a Base64 string to a UTF-8 string.
    /// </summary>
    /// <param name="base64String">The Base64 string to decode.</param>
    /// <returns>A UTF-8 string representation of the Base64 string.</returns>
    internal static string DecodeToUtf8(this string base64String) =>
        Encoding.UTF8.GetString(base64String.ToBytes());

    /// <summary>
    /// Determines whether the specified string is a valid Base64 string.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns><c>true</c> if the string is a valid Base64 string; otherwise, <c>false</c>.</returns>
    internal static bool IsBase64(this string input) =>
        input.Length % 4 == 0 && TryFromBase64String(input, new byte[input.Length], out _);

    /// <summary>
    /// Encodes a UTF-8 string to a Base64 string.
    /// </summary>
    /// <param name="input">The UTF-8 string to encode.</param>
    /// <returns>A Base64 string representation of the UTF-8 string.</returns>
    internal static string EncodeToBase64(this string input) =>
        ToBase64String(Encoding.UTF8.GetBytes(input));

    /// <summary>
    /// Decodes a Base64 URL string to a byte array.
    /// </summary>
    /// <param name="base64Url">The Base64 URL string to decode.</param>
    /// <returns>A byte array representation of the Base64 URL string.</returns>
    internal static byte[] Decode(this string base64Url) =>
        base64Url.RemoveWhiteSpaces().ToBase64().ToBytes();

    /// <summary>
    /// Removes all white spaces from the specified string.
    /// </summary>
    /// <param name="input">The string to remove white spaces from.</param>
    /// <returns>The string without white spaces.</returns>
    private static string RemoveWhiteSpaces(this string input) => input
        .Replace("\r", "")
        .Replace("\n", "")
        .Replace(" ", "");

    /// <summary>
    /// Converts a Base64 URL string to a standard Base64 string.
    /// </summary>
    /// <param name="input">The Base64 URL string to convert.</param>
    /// <returns>A standard Base64 string representation of the Base64 URL string.</returns>
    private static string ToBase64(this string input) => input
        .PadRight(input.Length + (4 - input.Length % 4) % 4, '=')
        .Replace("_", "/")
        .Replace("-", "+");
}
