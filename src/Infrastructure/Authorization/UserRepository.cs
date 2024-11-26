using Abstractions.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

/// <summary>
/// Provides methods for managing users in the authorization context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRepository"/> class.
/// </remarks>
/// <param name="contextFactory">The context factory to create the authorization context.</param>
public sealed class UserRepository(IDbContextFactory<AuthorizationContext> contextFactory) : IUserRepository
{
    /// <summary>
    /// Retrieves all users associated with a specific role.
    /// </summary>
    /// <param name="role">The role to filter users by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of users.</returns>
    public async Task<IEnumerable<User>> UsersOf(Role role)
    {
        await using var context = CreateContext();
        var validRole = await GetRoleOrThrow(context, role);

        var users = await context.Users.AsNoTracking().Include(it => it.Role)
            .Where(it => it.Role.Name == validRole.Name).ToListAsync();

        return users.Select(Mapping);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="passwordHash">The hashed password of the new user.</param>
    /// <param name="role">The role assigned to the new user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
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

    /// <summary>
    /// Retrieves a user by their username and password hash.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="passwordHash">The hashed password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    public async Task<User> GetBy(string username, string passwordHash)
    {
        await using var context = CreateContext();
        var user = await VerifyCredential(context, username, passwordHash);

        return Mapping(user);
    }

    /// <summary>
    /// Changes the password of a user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="passwordHash">The current hashed password of the user.</param>
    /// <param name="newPasswordHash">The new hashed password of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ChangePassword(string username, string passwordHash, string newPasswordHash)
    {
        await using var context = CreateContext();
        var target = await VerifyCredential(context, username, passwordHash);

        target.PasswordHash = newPasswordHash;
        context.Users.Update(target);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Delete(User user)
    {
        await using var context = CreateContext();
        var target = await GetUserOrThrow(context, user.Name);

        context.Users.Remove(target);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures that there is no duplication of the username.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="name">The username to check for duplication.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="EntityDuplicateException">Thrown when a user with the same username already exists.</exception>
    private static async Task EnsureNoDuplication(AuthorizationContext context, string name)
    {
        if (await context.Users.AnyAsync(it => it.Name == name))
        {
            throw new EntityDuplicateException(name);
        }
    }

    /// <summary>
    /// Verifies the credentials of a user.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="username">The username of the user.</param>
    /// <param name="passwordHash">The hashed password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user model.</returns>
    /// <exception cref="PasswordMismatchException">Thrown when the password hash does not match.</exception>
    private static async Task<UserModel> VerifyCredential(
        AuthorizationContext context, string username, string passwordHash)
    {
        var user = await GetUserOrThrow(context, username);
        return user.PasswordHash == passwordHash ? user : throw new PasswordMismatchException();
    }

    /// <summary>
    /// Retrieves a user by their username or throws an exception if not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="username">The username of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user model.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the user is not found.</exception>
    private static async Task<UserModel> GetUserOrThrow(AuthorizationContext context, string username)
    {
        var user = await context.Users.Include(it => it.Role)
            .FirstOrDefaultAsync(it => it.Name == username);
        return user ?? throw new EntityNotFoundException(username);
    }

    /// <summary>
    /// Retrieves a role by its name or throws an exception if not found.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="role">The role to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role model.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the role is not found.</exception>
    private static async Task<RoleModel> GetRoleOrThrow(AuthorizationContext context, Role role)
    {
        var model = await context.Roles.SingleOrDefaultAsync(it => it.Name == role.Name);
        return model ?? throw new EntityNotFoundException(role.Name);
    }

    /// <summary>
    /// Maps a user model to a user.
    /// </summary>
    /// <param name="from">The user model to map from.</param>
    /// <returns>The mapped user.</returns>
    private static User Mapping(UserModel from) => new(from.Name, new Role(from.Role.Name));

    /// <summary>
    /// Creates a new authorization context.
    /// </summary>
    /// <returns>The created authorization context.</returns>
    private AuthorizationContext CreateContext() => contextFactory.CreateDbContext();
}
