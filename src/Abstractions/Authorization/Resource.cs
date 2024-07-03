namespace Abstractions.Authorization;

public sealed record Resource(
    string Name,
    string Category,
    string Identifier,
    AccessControlMode Mode)
{
    public IEnumerable<string> GrantedRoleNames { get; init; } = [];

    public bool IsAccessibleTo(Role role) =>
        GrantedRoleNames.Contains(role.Name);
}