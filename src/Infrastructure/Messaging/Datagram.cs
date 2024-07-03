namespace Infrastructure.Messaging;

internal sealed record Datagram(
    string Topic,
    string Message);