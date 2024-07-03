namespace Infrastructure.UnitTests.Authorization;

[Category(ExternalDependency)]
internal sealed class ResourceRepositoryTests
{
    private const string Name = "NuclearLaunchButton";
    private const string Category = "Danger";
    private const string Identifier = "Danger/NuclearLaunchButton";
    private const AccessControlMode Mode = AccessControlMode.Disabled;

    private readonly IDbContextFactory<AuthorizationContext> _factory = AuthorizationContextFactory();
    private IResourceRepository _repository = default!;
    private readonly Role _role = Role.SuperAdministrator;

    [SetUp]
    public async Task Setup()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.Roles.Add(new RoleModel(_role.Name));
        await context.SaveChangesAsync();

        _repository = new ResourceRepository(_factory);
    }

    [TestCase("")]
    [TestCase(" ")]
    public async Task ShouldThrowExceptionWhenCreateResourceWithEmptyIdentifier(string identifier)
    {
        var create = () => _repository.Create(Name, Category, identifier, Mode);

        await create.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ShouldCreateResourceCorrectlyWhenGivenValidFields()
    {
        await _repository.Create(Name, Category, Identifier, Mode);

        var resources = await _repository.Resources();
        resources.Should().Contain(it => it.Identifier == Identifier && it.Mode == Mode);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenCreateWithDuplicateIdentifier()
    {
        await _repository.Create("Alpha", "Alpha", Identifier, Mode);

        var create = () => _repository.Create("Beta", "Beta", Identifier, Mode);

        await create.Should().ThrowAsync<EntityDuplicateException>();
    }

    [Test]
    public async Task ShouldGetAllResourcesBeenAdded()
    {
        await _repository.Create(Name, Category, Identifier, Mode);

        var resources = await _repository.Resources();

        resources.Should().Contain(it => it.Identifier == Identifier);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenTryGetResourceNotExist()
    {
        var query = () => _repository.GetBy("NotExistResource");

        await query.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldGetResourceCorrectlyWhenQueryWithRightIdentifier()
    {
        await _repository.Create(Name, Category, Identifier, Mode);

        var resource = await _repository.GetBy(Identifier);

        resource.Identifier.Should().Be(Identifier);
        resource.Mode.Should().Be(Mode);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGrantPermissionWithResourceNotExist()
    {
        await _repository.Create(Name, Category, Identifier, Mode);
        var resourceNotExist = new Resource(Name, Category, "NotExist", Mode);

        var grantPermission = () => _repository.GrantPermission(resourceNotExist, [_role]);

        await grantPermission.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenGrantPermissionWithRoleNotExist()
    {
        var resource = await _repository.Create(Name, Category, Identifier, Mode);

        var grantPermission = () => _repository.GrantPermission(resource, [new Role("NotExist")]);

        await grantPermission.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ShouldGrantPermissionCorrectlyWithRightResourceAndRoles()
    {
        var resource = await _repository.Create(Name, Category, Identifier, Mode);
        resource.GrantedRoleNames.Should().BeEmpty();

        resource = await _repository.GrantPermission(resource, [_role]);

        resource.IsAccessibleTo(_role).Should().BeTrue();
    }

    [Test]
    public async Task ShouldDeleteResourceCorrectly()
    {
        var resource = await _repository.Create(Name, Category, Identifier, Mode);
        (await _repository.Resources()).Should().ContainSingle();

        await _repository.Delete(resource);

        (await _repository.Resources()).Should().BeEmpty();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeleteNotExistResource()
    {
        var notExistResource = new Resource(Name, Category, "NotExist", Mode);

        var delete = () => _repository.Delete(notExistResource);

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