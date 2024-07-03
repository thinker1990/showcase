namespace Abstractions.Authorization;

public sealed record User(
    string Name,
    Role Role);