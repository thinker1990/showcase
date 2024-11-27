namespace Infrastructure.Configuration;

/// <summary>
/// Represents a configuration setting with a key and value.
/// </summary>
/// <param name="Key">The key of the configuration setting.</param>
/// <param name="Value">The value of the configuration setting.</param>
internal sealed record Settings(string Key, string Value)
{
    /// <summary>
    /// Gets or sets the value of the configuration setting.
    /// </summary>
    public string Value { get; set; } = Value;
}
