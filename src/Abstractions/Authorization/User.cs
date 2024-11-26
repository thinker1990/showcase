namespace Abstractions.Authorization;

/// <summary>
/// Represents a user with a role.
/// </summary>
/// <param name="Name">The name of the user.</param>
/// <param name="Role">The role assigned to the user.</param>
public sealed record User(
   string Name,
   Role Role);
