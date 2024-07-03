using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public sealed class UserRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IUserRepository
{
    public async Task<IEnumerable<User>> UsersOf(Role role)
    {
        await using var context = CreateContext();
        var validRole = await GetRoleOrThrow(context, role);

        var users = await context.Users.AsNoTracking().Include(it => it.Role)
            .Where(it => it.Role.Name == validRole.Name).ToListAsync();

        return users.Select(Mapping);
    }

    public async Task<User> Create(string username, string passwordHash, Role role)
    {
        EnsureNotEmpty(username, nameof(username));
        EnsureNotEmpty(passwordHash, nameof(passwordHash));

        await using var context = CreateContext();
        await EnsureNoDuplication(context, username);

        var validRole = await GetRoleOrThrow(context, role);
        var user = new UserModel(username, passwordHash, validRole);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Mapping(user);
    }

    public async Task<User> GetBy(string username, string passwordHash)
    {
        await using var context = CreateContext();
        var user = await VerifyCredential(context, username, passwordHash);

        return Mapping(user);
    }

    public async Task ChangePassword(string username, string passwordHash, string newPasswordHash)
    {
        await using var context = CreateContext();
        var target = await VerifyCredential(context, username, passwordHash);

        target.PasswordHash = newPasswordHash;
        context.Users.Update(target);
        await context.SaveChangesAsync();
    }

    public async Task Delete(User user)
    {
        await using var context = CreateContext();
        var target = await GetUserOrThrow(context, user.Name);

        context.Users.Remove(target);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureNoDuplication(AuthorizationContext context, string name)
    {
        if (await context.Users.AnyAsync(it => it.Name == name))
        {
            throw new EntityDuplicateException(name);
        }
    }

    private static async Task<UserModel> VerifyCredential(
        AuthorizationContext context, string username, string passwordHash)
    {
        var user = await GetUserOrThrow(context, username);
        return user.PasswordHash == passwordHash ? user : throw new PasswordMismatchException();
    }

    private static async Task<UserModel> GetUserOrThrow(AuthorizationContext context, string username)
    {
        var user = await context.Users.Include(it => it.Role)
            .FirstOrDefaultAsync(it => it.Name == username);
        return user ?? throw new EntityNotFoundException(username);
    }

    private static async Task<RoleModel> GetRoleOrThrow(AuthorizationContext context, Role role)
    {
        var model = await context.Roles.SingleOrDefaultAsync(it => it.Name == role.Name);
        return model ?? throw new EntityNotFoundException(role.Name);
    }

    private static User Mapping(UserModel from) => new(from.Name, new Role(from.Role.Name));

    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}