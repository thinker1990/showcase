namespace Infrastructure.Authorization;

internal sealed record UserModel()
{
    public string Id { get; init; } = UniqueId();

    public string Name { get; init; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public RoleModel Role { get; init; } = default!;

    public DateTime Created { get; init; } = DateTime.Now;

    public UserModel(string name, string passwordHash, RoleModel role)
        : this()
    {
        Name = name;
        PasswordHash = passwordHash;
        Role = role;
    }
}