using System.Text;
using static System.Convert;

namespace Infrastructure.Activation;

internal static class Base64Extensions
{
    internal static byte[] ToBytes(this string base64String) =>
        FromBase64String(base64String);

    internal static string DecodeToUtf8(this string base64String) =>
        Encoding.UTF8.GetString(base64String.ToBytes());

    internal static bool IsBase64(this string input) =>
        input.Length % 4 == 0 && TryFromBase64String(input, new byte[input.Length], out _);

    internal static string EncodeToBase64(this string input) =>
        ToBase64String(Encoding.UTF8.GetBytes(input));

    internal static byte[] Decode(this string base64Url) =>
        base64Url.RemoveWhiteSpaces().ToBase64().ToBytes();

    private static string RemoveWhiteSpaces(this string input) => input
        .Replace("\r", "")
        .Replace("\n", "")
        .Replace(" ", "");

    private static string ToBase64(this string input) => input
        .PadRight(input.Length + (4 - input.Length % 4) % 4, '=')
        .Replace("_", "/")
        .Replace("-", "+");
}