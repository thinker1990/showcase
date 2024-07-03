namespace Infrastructure.Authorization;

internal sealed record RoleModel()
{
    public string Id { get; init; } = UniqueId();

    public string Name { get; init; } = string.Empty;

    public List<ResourceModel> AccessibleResources { get; set; } = [];

    public RoleModel(string name)
        : this()
    {
        Name = name;
    }
}