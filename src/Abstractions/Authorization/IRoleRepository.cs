namespace Abstractions.Authorization;

/// <summary>
/// Defines the contract for a repository that manages roles.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of roles.</returns>
    Task<IEnumerable<Role>> Roles();

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created role.</returns>
    Task<Role> Create(string name);

    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    Task<Role> GetBy(string name);

    /// <summary>
    /// Grants permission to a role for the specified resources.
    /// </summary>
    /// <param name="role">The role to grant permission to.</param>
    /// <param name="resources">The resources to grant permission to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated role.</returns>
    Task<Role> GrantPermission(Role role, IEnumerable<Resource> resources);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="role">The role to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Delete(Role role);
}
