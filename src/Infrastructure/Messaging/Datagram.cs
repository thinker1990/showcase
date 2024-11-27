namespace Infrastructure.Messaging;

/// <summary>
/// Represents a datagram with a topic and message.
/// </summary>
/// <param name="Topic">The topic of the datagram.</param>
/// <param name="Message">The message of the datagram.</param>
internal sealed record Datagram(
    string Topic,
    string Message);
