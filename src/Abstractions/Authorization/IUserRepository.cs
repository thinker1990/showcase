namespace Abstractions.Authorization;

public interface IUserRepository
{
    Task<IEnumerable<User>> UsersOf(Role role);

    Task<User> Create(string username, string passwordHash, Role role);

    Task<User> GetBy(string username, string passwordHash);

    Task ChangePassword(string username, string passwordHash, string newPasswordHash);

    Task Delete(User user);
}