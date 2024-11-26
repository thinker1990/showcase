namespace Abstractions.Authorization;

/// <summary>
/// Defines the contract for a repository that manages resources.
/// </summary>
public interface IResourceRepository
{
    /// <summary>
    /// Retrieves all resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of resources.</returns>
    Task<IEnumerable<Resource>> Resources();

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="category">The category of the resource.</param>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <param name="mode">The access control mode of the resource.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created resource.</returns>
    Task<Resource> Create(string name, string category, string identifier, AccessControlMode mode);

    /// <summary>
    /// Retrieves a resource by its identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the resource.</returns>
    Task<Resource> GetBy(string identifier);

    /// <summary>
    /// Grants permission to a resource for the specified roles.
    /// </summary>
    /// <param name="resource">The resource to grant permission to.</param>
    /// <param name="roles">The roles to grant permission to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated resource.</returns>
    Task<Resource> GrantPermission(Resource resource, IEnumerable<Role> roles);

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    /// <param name="resource">The resource to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Delete(Resource resource);
}
