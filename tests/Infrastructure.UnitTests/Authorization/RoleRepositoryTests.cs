namespace Infrastructure.UnitTests.Authorization;

[Category(ExternalDependency)]
internal sealed class RoleRepositoryTests
{
    private static string RoleName => Role.Administrator.Name;
    private static Resource Resource => new("Resource", "Category", "Identifier", AccessControlMode.Visible);

    private readonly IDbContextFactory<AuthorizationContext> _factory = AuthorizationContextFactory();
    private IRoleRepository _repository = default!;

    [SetUp]
    public async Task Setup()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        _repository = new RoleRepository(_factory);
    }

    [TestCase("")]
    [TestCase(" ")]
    public async Task ShouldThrowExceptionWhenCreateRoleWithEmptyName(string name)
    {
        var create = () => _repository.Create(name);

        await create.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ShouldCreateRoleCorrectlyWhenGivenValidName()
    {
        await _repository.Create(RoleName);

        var roles = await _repository.Roles();
        roles.Should().Contain(it => it.Name == RoleName);
    }

    [Test]
    public async Task ShouldGetAllRolesBeenAdded()
    {
        await _repository.Create(Role.Operator.Name);
        await _repository.Create(Role.Administrator.Name);
        await _repository.Create(Role.SuperAdministrator.Name);

        (await _repository.Roles()).Should().HaveCount(3);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenCreateDuplicateRoles()
    {
        await _repository.Create(RoleName);

        var create = () => _repository.Create(RoleName);

        await create.Should().ThrowAsync<EntityDuplicateException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenTryGetRoleNotExist()
    {
        var query = () => _repository.GetBy("NotExistRole");

        await query.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldGetRoleCorrectlyWhenQueryWithNameThatExist()
    {
        await _repository.Create(RoleName);

        var role = await _repository.GetBy(RoleName);

        role.Name.Should().Be(RoleName);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGrantPermissionWithRoleNotExist()
    {
        var roleNotExist = new Role("NotExist");

        var grantPermission = () => _repository.GrantPermission(roleNotExist, []);

        await grantPermission.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGrantPermissionWithResourceNotExist()
    {
        var role = await _repository.Create(RoleName);

        var grantPermission = () => _repository.GrantPermission(role, [Resource]);

        await grantPermission.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldGrantPermissionCorrectlyWithRightResourceAndRoles()
    {
        await Preload(Resource);
        var role = await _repository.Create(RoleName);

        role = await _repository.GrantPermission(role, [Resource]);

        role.HaveAccessTo(Resource).Should().BeTrue();
    }

    [Test]
    public async Task ShouldDeleteRoleCorrectly()
    {
        var role = await _repository.Create(RoleName);
        (await _repository.Roles()).Should().ContainSingle();

        await _repository.Delete(role);

        (await _repository.Roles()).Should().BeEmpty();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeleteNotExistRole()
    {
        var delete = () => _repository.Delete(new Role("NotExist"));

        await delete.Should().ThrowAsync<EntityNotFoundException>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }

    private async Task Preload(Resource resource)
    {
        await using var context = CreateContext();
        var model = new ResourceModel(resource.Name, resource.Category, resource.Identifier, resource.Mode);
        context.Resources.Add(model);

        await context.SaveChangesAsync();
    }

    private AuthorizationContext CreateContext() => _factory.CreateDbContext();
}