using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public sealed class ResourceRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IResourceRepository
{
    public async Task<IEnumerable<Resource>> Resources()
    {
        await using var context = CreateContext();
        var resources = await context.Resources.AsNoTracking()
            .Include(it => it.GrantedRoles).ToListAsync();

        return resources.Select(Mapping);
    }

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

    public async Task<Resource> GetBy(string identifier)
    {
        await using var context = CreateContext();
        var resource = await GetResourceOrThrow(context, identifier);

        return Mapping(resource);
    }

    public async Task<Resource> GrantPermission(Resource resource, IEnumerable<Role> roles)
    {
        await using var context = CreateContext();
        var target = await GetResourceOrThrow(context, resource.Identifier);
        target.GrantedRoles = await GetRolesOrThrow(context, roles);

        context.Resources.Update(target);
        await context.SaveChangesAsync();

        return Mapping(target);
    }

    public async Task Delete(Resource resource)
    {
        await using var context = CreateContext();
        var target = await GetResourceOrThrow(context, resource.Identifier);

        context.Resources.Remove(target);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureNoDuplication(AuthorizationContext context, string identifier)
    {
        if (await context.Resources.AnyAsync(it => it.Identifier == identifier))
        {
            throw new EntityDuplicateException(identifier);
        }
    }

    private static async Task<ResourceModel> GetResourceOrThrow(AuthorizationContext context, string identifier)
    {
        var resource = await context.Resources.Include(it => it.GrantedRoles)
            .SingleOrDefaultAsync(it => it.Identifier == identifier);
        return resource ?? throw new EntityNotFoundException(identifier);
    }

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

    private static Resource Mapping(ResourceModel from) =>
        new(from.Name, from.Category, from.Identifier, from.Mode)
        {
            GrantedRoleNames = from.GrantedRoles.Select(it => it.Name)
        };

    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}