namespace Infrastructure.Configuration;

internal sealed record Settings(string Key, string Value)
{
    public string Value { get; set; } = Value;
}