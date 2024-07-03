namespace Abstractions.Authorization;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> Roles();

    Task<Role> Create(string name);

    Task<Role> GetBy(string name);

    Task<Role> GrantPermission(Role role, IEnumerable<Resource> resources);

    Task Delete(Role role);
}