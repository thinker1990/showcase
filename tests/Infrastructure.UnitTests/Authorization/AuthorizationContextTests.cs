namespace Infrastructure.UnitTests.Authorization;

[Category(ExternalDependency)]
internal sealed class AuthorizationContextTests
{
    private readonly IDbContextFactory<AuthorizationContext> _factory = AuthorizationContextFactory();
    private AuthorizationContext _context = default!;

    [SetUp]
    public async Task Setup()
    {
        _context = await _factory.CreateDbContextAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    [Test]
    public void DataSetsShouldNotBeNull()
    {
        _context.Roles.Should().NotBeNull();
        _context.Users.Should().NotBeNull();
        _context.Resources.Should().NotBeNull();
    }

    [TestCase("Admin")]
    public async Task ShouldAddRoleCorrectly(string roleName)
    {
        var role = new RoleModel(roleName);

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        _context.Roles.Should().Contain(it => it.Name == roleName);
    }

    [TestCase("Mike", "Operator")]
    public async Task ShouldAddUserCorrectly(string userName, string roleName)
    {
        var role = new RoleModel(roleName);
        var user = new UserModel(userName, "PasswordHash", role);

        _context.Roles.Add(role);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _context.Users.Should().Contain(it => it.Name == userName && it.Role.Name == roleName);
    }

    [TestCase("NuclearLaunchButton", "SuperAdmin")]
    public async Task ShouldAddResourceCorrectly(string resourceName, string roleName)
    {
        var role = new RoleModel(roleName);
        var resource = new ResourceModel(resourceName, "category", "identifier", AccessControlMode.Hidden)
        {
            GrantedRoles = [role]
        };

        _context.Roles.Add(role);
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        _context.Resources.Should().Contain(it => it.Name == resourceName && it.GrantedRoles.Contains(role));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }
}