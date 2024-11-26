namespace Abstractions.Authorization;

/// <summary>
/// Represents a resource with access control.
/// </summary>
/// <param name="Name">The name of the resource.</param>
/// <param name="Category">The category of the resource.</param>
/// <param name="Identifier">The unique identifier of the resource.</param>
/// <param name="Mode">The access control mode of the resource.</param>
public sealed record Resource(
    string Name,
    string Category,
    string Identifier,
    AccessControlMode Mode)
{
    /// <summary>
    /// Gets the names of the roles that are granted access to the resource.
    /// </summary>
    public IEnumerable<string> GrantedRoleNames { get; init; } = [];

    /// <summary>
    /// Determines whether the resource is accessible to the specified role.
    /// </summary>
    /// <param name="role">The role to check access for.</param>
    /// <returns><c>true</c> if the resource is accessible to the role; otherwise, <c>false</c>.</returns>
    public bool IsAccessibleTo(Role role) =>
        GrantedRoleNames.Contains(role.Name);
}
