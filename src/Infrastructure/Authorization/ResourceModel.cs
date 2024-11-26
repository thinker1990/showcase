using Abstractions.Authorization;

namespace Infrastructure.Authorization;

/// <summary>
/// Represents a resource with access control.
/// </summary>
internal sealed record ResourceModel()
{
    /// <summary>
    /// Gets the unique identifier of the resource.
    /// </summary>
    public string Id { get; init; } = UniqueId();

    /// <summary>
    /// Gets the name of the resource.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the category of the resource.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier of the resource.
    /// </summary>
    public string Identifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets the access control mode of the resource.
    /// </summary>
    public AccessControlMode Mode { get; init; } = AccessControlMode.Disabled;

    /// <summary>
    /// Gets or sets the roles that are granted access to the resource.
    /// </summary>
    public List<RoleModel> GrantedRoles { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceModel"/> class with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="category">The category of the resource.</param>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <param name="mode">The access control mode of the resource.</param>
    public ResourceModel(
        string name,
        string category,
        string identifier,
        AccessControlMode mode)
        : this()
    {
        Name = name;
        Category = category;
        Identifier = identifier;
        Mode = mode;
    }
}
