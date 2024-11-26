namespace Infrastructure.Authorization;

/// <summary>
/// Represents a user with access control capabilities.
/// </summary>
internal sealed record UserModel()
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    public string Id { get; init; } = UniqueId();

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash of the user.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets the role of the user.
    /// </summary>
    public RoleModel Role { get; init; } = default!;

    /// <summary>
    /// Gets the creation date of the user.
    /// </summary>
    public DateTime Created { get; init; } = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserModel"/> class with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="passwordHash">The password hash of the user.</param>
    /// <param name="role">The role of the user.</param>
    public UserModel(string name, string passwordHash, RoleModel role)
        : this()
    {
        Name = name;
        PasswordHash = passwordHash;
        Role = role;
    }
}
