using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abstractions.Extensions;

public static class JsonExtensions
{
    public const string EmptyCollection = "[]";

    public const string EmptyObject = "{}";

    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static T Deserialize<T>(this string json)
    {
        Utility.EnsureNotEmpty(json, nameof(json));

        return JsonSerializer.Deserialize<T>(json, DefaultOptions)
               ?? throw new ArgumentException($"Invalid JSON: {json}.");
    }

    public static string Serialize<T>(this T entity) where T : notnull
    {
        return JsonSerializer.Serialize(entity, DefaultOptions);
    }
}