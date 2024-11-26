namespace Infrastructure.Authorization;

/// <summary>
/// Represents a role with access control capabilities.
/// </summary>
internal sealed record RoleModel()
{
    /// <summary>
    /// Gets the unique identifier of the role.
    /// </summary>
    public string Id { get; init; } = UniqueId();

    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the resources that are accessible to the role.
    /// </summary>
    public List<ResourceModel> AccessibleResources { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleModel"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    public RoleModel(string name)
        : this()
    {
        Name = name;
    }
}
