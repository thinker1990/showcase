namespace Abstractions.Authorization;

public interface IResourceRepository
{
    Task<IEnumerable<Resource>> Resources();

    Task<Resource> Create(string name, string category, string identifier, AccessControlMode mode);

    Task<Resource> GetBy(string identifier);

    Task<Resource> GrantPermission(Resource resource, IEnumerable<Role> roles);

    Task Delete(Resource resource);
}