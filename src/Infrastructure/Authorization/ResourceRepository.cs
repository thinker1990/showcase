using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

/// <summary>
/// Provides methods for managing resources in the authorization context.
/// </summary>
public sealed class ResourceRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IResourceRepository
{
    /// <summary>
    /// Retrieves all resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of resources.</returns>
    public async Task<IEnumerable<Resource>> Resources()
    {
        await using var context = CreateContext();
        var resources = await context.Resources.AsNoTracking()
            .Include(it => it.GrantedRoles).ToListAsync();

        return resources.Select(Mapping);
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="category">The category of the resource.</param>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <param name="mode">The access control mode of the resource.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created resource.</returns>
    public async Task<Resource> Create(
        string name, string category, string identifier, AccessControlMode mode)
    {
        EnsureNotEmpty(name, nameof(name));
        EnsureNotEmpty(category, nameof(category));
        EnsureNotEmpty(identifier, nameof(identifier));

        await using var context = CreateContext();
        await EnsureNoDuplication(context, identifier);

        var resource = new ResourceModel(name, category, identifier, mode);
        context.Resources.Add(resource);
        await context.SaveChangesAsync();
        return Mapping(resource);
    }

    /// <summary>
    /// Retrieves a resource by its identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the resource.</returns>
    public async Task<Resource> GetBy(string identifier)
    {
        await using var context = CreateContext();
        var resource = await GetResourceOrThrow(context, identifier);

        return Mapping(resource);
    }

    /// <summary>
    /// Grants permission to a resource for the specified roles.
    /// </summary>
    /// <param name="resource">The resource to grant permission to.</param>
    /// <param name="roles">The roles to grant permission to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated resource.</returns>
    public async Task<Resource> GrantPermission(Resource resource, IEnumerable<Role> roles)
    {
        await using var context = CreateContext();
        var target = await GetResourceOrThrow(context, resource.Identifier);
        target.GrantedRoles = await GetRolesOrThrow(context, roles);

        context.Resources.Update(target);
        await context.SaveChangesAsync();

        return Mapping(target);
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    /// <param name="resource">The resource to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Delete(Resource resource)
    {
        await using var context = CreateContext();
        var target = await GetResourceOrThrow(context, resource.Identifier);

        context.Resources.Remove(target);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures that there is no duplication of the resource identifier.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="EntityDuplicateException">Thrown when a resource with the same identifier already exists.</exception>
    private static async Task EnsureNoDuplication(AuthorizationContext context, string identifier)
    {
        if (await context.Resources.AnyAsync(it => it.Identifier == identifier))
        {
            throw new EntityDuplicateException(identifier);
        }
    }

    /// <summary>
    /// Retrieves a resource by its identifier or throws an exception if not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="identifier">The unique identifier of the resource.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the resource model.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the resource is not found.</exception>
    private static async Task<ResourceModel> GetResourceOrThrow(AuthorizationContext context, string identifier)
    {
        var resource = await context.Resources.Include(it => it.GrantedRoles)
            .SingleOrDefaultAsync(it => it.Identifier == identifier);
        return resource ?? throw new EntityNotFoundException(identifier);
    }

    /// <summary>
    /// Retrieves the roles by their names or throws an exception if any role is not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="roles">The roles to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of role models.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when a role is not found.</exception>
    private static async Task<List<RoleModel>> GetRolesOrThrow(AuthorizationContext context, IEnumerable<Role> roles)
    {
        var all = roles.Select(it => it.Name);
        var exists = await context.Roles.Where(it => all.Contains(it.Name)).ToListAsync();

        var unexpected = all.Except(exists.Select(it => it.Name)).FirstOrDefault();
        if (unexpected is not null)
        {
            throw new EntityNotFoundException(unexpected);
        }

        return exists;
    }

    /// <summary>
    /// Maps a resource model to a resource.
    /// </summary>
    /// <param name="from">The resource model to map from.</param>
    /// <returns>The mapped resource.</returns>
    private static Resource Mapping(ResourceModel from) =>
        new(from.Name, from.Category, from.Identifier, from.Mode)
        {
            GrantedRoleNames = from.GrantedRoles.Select(it => it.Name)
        };

    /// <summary>
    /// Creates a new authorization context.
    /// </summary>
    /// <returns>The created authorization context.</returns>
    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}
