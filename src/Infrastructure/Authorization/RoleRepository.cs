using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

/// <summary>
/// Provides methods for managing roles in the authorization context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleRepository"/> class.
/// </remarks>
/// <param name="contextFactory">The context factory to create the authorization context.</param>
public sealed class RoleRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IRoleRepository
{
    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of roles.</returns>
    public async Task<IEnumerable<Role>> Roles()
    {
        await using var context = CreateContext();
        var roles = await context.Roles.AsNoTracking()
            .Include(it => it.AccessibleResources).ToListAsync();

        return roles.Select(Mapping);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created role.</returns>
    public async Task<Role> Create(string name)
    {
        EnsureNotEmpty(name, nameof(name));

        await using var context = CreateContext();
        await EnsureNoDuplication(context, name);

        var role = new RoleModel(name);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        return Mapping(role);
    }

    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    public async Task<Role> GetBy(string name)
    {
        await using var context = CreateContext();
        var role = await GetRoleOrThrow(context, name);

        return Mapping(role);
    }

    /// <summary>
    /// Grants permission to a role for the specified resources.
    /// </summary>
    /// <param name="role">The role to grant permission to.</param>
    /// <param name="resources">The resources to grant permission to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated role.</returns>
    public async Task<Role> GrantPermission(Role role, IEnumerable<Resource> resources)
    {
        await using var context = CreateContext();
        var target = await GetRoleOrThrow(context, role.Name);
        target.AccessibleResources = await GetResourcesOrThrow(context, resources);

        context.Roles.Update(target);
        await context.SaveChangesAsync();

        return Mapping(target);
    }

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="role">The role to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Delete(Role role)
    {
        await using var context = CreateContext();
        var target = await GetRoleOrThrow(context, role.Name);

        context.Roles.Remove(target);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures that there is no duplication of the role name.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="EntityDuplicateException">Thrown when a role with the same name already exists.</exception>
    private static async Task EnsureNoDuplication(AuthorizationContext context, string name)
    {
        if (await context.Roles.AnyAsync(it => it.Name == name))
        {
            throw new EntityDuplicateException(name);
        }
    }

    /// <summary>
    /// Retrieves a role by its name or throws an exception if not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="name">The name of the role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role model.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the role is not found.</exception>
    private static async Task<RoleModel> GetRoleOrThrow(AuthorizationContext context, string name)
    {
        var role = await context.Roles.Include(it => it.AccessibleResources)
            .SingleOrDefaultAsync(it => it.Name == name);

        return role ?? throw new EntityNotFoundException(name);
    }

    /// <summary>
    /// Retrieves the resources by their identifiers or throws an exception if any resource is not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="resources">The resources to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of resource models.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when a resource is not found.</exception>
    private static async Task<List<ResourceModel>> GetResourcesOrThrow(
        AuthorizationContext context, IEnumerable<Resource> resources)
    {
        var all = resources.Select(it => it.Identifier);
        var exists = await context.Resources.Where(it => all.Contains(it.Identifier)).ToListAsync();

        var unexpected = all.Except(exists.Select(it => it.Identifier)).FirstOrDefault();
        if (unexpected is not null)
        {
            throw new EntityNotFoundException(unexpected);
        }

        return exists;
    }

    /// <summary>
    /// Maps a role model to a role.
    /// </summary>
    /// <param name="from">The role model to map from.</param>
    /// <returns>The mapped role.</returns>
    private static Role Mapping(RoleModel from) => new(from.Name)
    {
        AccessibleResourceIdentifiers = from.AccessibleResources.Select(it => it.Identifier)
    };

    /// <summary>
    /// Creates a new authorization context.
    /// </summary>
    /// <returns>The created authorization context.</returns>
    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}
