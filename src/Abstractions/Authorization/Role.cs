namespace Abstractions.Authorization;

/// <summary>
/// Represents a role with access control capabilities.
/// </summary>
/// <param name="Name">The name of the role.</param>
public sealed record Role(string Name)
{
    /// <summary>
    /// Gets the identifiers of the resources that are accessible to the role.
    /// </summary>
    public IEnumerable<string> AccessibleResourceIdentifiers { get; init; } = [];

    /// <summary>
    /// Determines whether the role has access to the specified resource.
    /// </summary>
    /// <param name="resource">The resource to check access for.</param>
    /// <returns><c>true</c> if the role has access to the resource; otherwise, <c>false</c>.</returns>
    public bool HaveAccessTo(Resource resource) =>
        AccessibleResourceIdentifiers.Contains(resource.Identifier);

    /// <summary>
    /// Gets the operator role.
    /// </summary>
    public static Role Operator => new(nameof(Operator));

    /// <summary>
    /// Gets the administrator role.
    /// </summary>
    public static Role Administrator => new(nameof(Administrator));

    /// <summary>
    /// Gets the super administrator role.
    /// </summary>
    public static Role SuperAdministrator => new(nameof(SuperAdministrator));
}
