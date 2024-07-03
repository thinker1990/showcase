using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public sealed class RoleRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IRoleRepository
{
    public async Task<IEnumerable<Role>> Roles()
    {
        await using var context = CreateContext();
        var roles = await context.Roles.AsNoTracking()
            .Include(it => it.AccessibleResources).ToListAsync();

        return roles.Select(Mapping);
    }

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

    public async Task<Role> GetBy(string name)
    {
        await using var context = CreateContext();
        var role = await GetRoleOrThrow(context, name);

        return Mapping(role);
    }

    public async Task<Role> GrantPermission(Role role, IEnumerable<Resource> resources)
    {
        await using var context = CreateContext();
        var target = await GetRoleOrThrow(context, role.Name);
        target.AccessibleResources = await GetResourcesOrThrow(context, resources);

        context.Roles.Update(target);
        await context.SaveChangesAsync();

        return Mapping(target);
    }

    public async Task Delete(Role role)
    {
        await using var context = CreateContext();
        var target = await GetRoleOrThrow(context, role.Name);

        context.Roles.Remove(target);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureNoDuplication(AuthorizationContext context, string name)
    {
        if (await context.Roles.AnyAsync(it => it.Name == name))
        {
            throw new EntityDuplicateException(name);
        }
    }

    private static async Task<RoleModel> GetRoleOrThrow(AuthorizationContext context, string name)
    {
        var role = await context.Roles.Include(it => it.AccessibleResources)
            .SingleOrDefaultAsync(it => it.Name == name);

        return role ?? throw new EntityNotFoundException(name);
    }

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

    private static Role Mapping(RoleModel from) => new(from.Name)
    {
        AccessibleResourceIdentifiers = from.AccessibleResources.Select(it => it.Identifier)
    };

    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}