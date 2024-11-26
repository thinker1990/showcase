using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abstractions.Extensions;

/// <summary>
/// Provides extension methods for JSON serialization and deserialization.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Represents an empty JSON array.
    /// </summary>
    public const string EmptyCollection = "[]";

    /// <summary>
    /// Represents an empty JSON object.
    /// </summary>
    public const string EmptyObject = "{}";

    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the JSON string is invalid.</exception>
    public static T Deserialize<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));
        }

        return JsonSerializer.Deserialize<T>(json, DefaultOptions)
               ?? throw new ArgumentException($"Invalid JSON: {json}.");
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="entity">The object to serialize.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string Serialize<T>(this T entity) where T : notnull
    {
        return JsonSerializer.Serialize(entity, DefaultOptions);
    }
}
