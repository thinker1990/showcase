namespace Abstractions.Authorization;

public sealed record Role(string Name)
{
    public IEnumerable<string> AccessibleResourceIdentifiers { get; init; } = [];

    public bool HaveAccessTo(Resource resource) =>
        AccessibleResourceIdentifiers.Contains(resource.Identifier);

    public static Role Operator => new(nameof(Operator));

    public static Role Administrator => new(nameof(Administrator));

    public static Role SuperAdministrator => new(nameof(SuperAdministrator));
}