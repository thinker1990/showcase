using Abstractions.Authorization;

namespace Infrastructure.Authorization;

internal sealed record ResourceModel()
{
    public string Id { get; init; } = UniqueId();

    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public AccessControlMode Mode { get; init; } = AccessControlMode.Disabled;

    public List<RoleModel> GrantedRoles { get; set; } = [];

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