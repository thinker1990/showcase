namespace Abstractions.Authorization;

/// <summary>
/// Defines the contract for a repository that manages users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves all users associated with a specific role.
    /// </summary>
    /// <param name="role">The role to filter users by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of users.</returns>
    Task<IEnumerable<User>> UsersOf(Role role);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="passwordHash">The hashed password of the new user.</param>
    /// <param name="role">The role assigned to the new user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    Task<User> Create(string username, string passwordHash, Role role);

    /// <summary>
    /// Retrieves a user by their username and password hash.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="passwordHash">The hashed password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    Task<User> GetBy(string username, string passwordHash);

    /// <summary>
    /// Changes the password of a user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="passwordHash">The current hashed password of the user.</param>
    /// <param name="newPasswordHash">The new hashed password of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ChangePassword(string username, string passwordHash, string newPasswordHash);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Delete(User user);
}
