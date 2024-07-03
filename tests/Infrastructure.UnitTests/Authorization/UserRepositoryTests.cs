namespace Infrastructure.UnitTests.Authorization;

[Category(ExternalDependency)]
internal sealed class UserRepositoryTests
{
    private const string Username = "Mike";
    private const string PasswordHash = "FakePasswordHash";

    private readonly IDbContextFactory<AuthorizationContext> _factory = AuthorizationContextFactory();
    private IUserRepository _repository = default!;
    private readonly Role _role = Role.Operator;

    [SetUp]
    public async Task Setup()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.Roles.Add(new RoleModel(_role.Name));
        await context.SaveChangesAsync();

        _repository = new UserRepository(_factory);
    }

    [TestCase("")]
    [TestCase(" ")]
    public async Task ShouldThrowExceptionWhenCreateUserWithEmptyName(string name)
    {
        var create = () => _repository.Create(name, PasswordHash, _role);

        await create.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenCreateUserWithNotExistRole()
    {
        var create = () => _repository.Create(Username, PasswordHash, new Role("NotExist"));

        await create.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldCreateUserCorrectlyWhenGivenValidNameAndRole()
    {
        await _repository.Create(Username, PasswordHash, _role);

        var users = await _repository.UsersOf(_role);
        users.Should().Contain(it => it.Name == Username);
    }

    [Test]
    public async Task ShouldGetUsersOfCorrectRole()
    {
        await _repository.Create(Username, PasswordHash, _role);

        var users = await _repository.UsersOf(_role);

        users.Should().Contain(it => it.Role.Name == _role.Name);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGetUsersOfNotExistRole()
    {
        var getUsers = () => _repository.UsersOf(new Role("NotExist"));

        await getUsers.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenCreateDuplicateUsers()
    {
        await _repository.Create(Username, PasswordHash, _role);

        var create = () => _repository.Create(Username, PasswordHash, _role);

        await create.Should().ThrowAsync<EntityDuplicateException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenTryGetUserNotExist()
    {
        var query = () => _repository.GetBy("NotExistUser", PasswordHash);

        await query.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenTryGetUserWithWrongPassword()
    {
        await _repository.Create(Username, PasswordHash, _role);

        var query = () => _repository.GetBy(Username, "WrongPasswordHash");

        await query.Should().ThrowAsync<PasswordMismatchException>();
    }

    [Test]
    public async Task ShouldGetUserCorrectlyWhenQueryWithRightNameAndPassword()
    {
        await _repository.Create(Username, PasswordHash, _role);

        var user = await _repository.GetBy(Username, PasswordHash);

        user.Name.Should().Be(Username);
    }

    [TestCase("WrongUsername", PasswordHash)]
    [TestCase(Username, "WrongPasswordHash")]
    [TestCase("WrongUsername", "WrongPasswordHash")]
    public async Task ShouldThrowExceptionWhenChangePasswordWithWrongNameOrPassword(string username, string passwordHash)
    {
        await _repository.Create(Username, PasswordHash, _role);

        var changePassword = () => _repository.ChangePassword(username, passwordHash, "NewPasswordHash");

        await changePassword.Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task ShouldChangePasswordCorrectlyWhenGivenRightNameAndPassword()
    {
        await _repository.Create(Username, PasswordHash, _role);
        const string newPasswordHash = "NewPasswordHash";

        await _repository.ChangePassword(Username, PasswordHash, newPasswordHash);

        await using var context = CreateContext();
        context.Users.Should().Contain(it => it.Name == Username && it.PasswordHash == newPasswordHash);
    }

    [Test]
    public async Task ShouldDeleteUserCorrectly()
    {
        var user = await _repository.Create(Username, PasswordHash, _role);
        (await _repository.UsersOf(_role)).Should().ContainSingle();

        await _repository.Delete(user);

        (await _repository.UsersOf(_role)).Should().BeEmpty();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeleteNotExistUser()
    {
        var notExistUser = new User("NotExist", _role);

        var delete = () => _repository.Delete(notExistUser);

        await delete.Should().ThrowAsync<EntityNotFoundException>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }

    private AuthorizationContext CreateContext() => _factory.CreateDbContext();
}