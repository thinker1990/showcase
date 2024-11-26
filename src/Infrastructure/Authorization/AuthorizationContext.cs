using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

/// <summary>
/// Represents the authorization context for the application.
/// </summary>
/// <param name="options">The options to configure the context.</param>
public sealed class AuthorizationContext(DbContextOptions<AuthorizationContext> options) : DbContext(options)
{
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    /// <summary>
    /// Gets or sets the DbSet for roles.
    /// </summary>
    private DbSet<RoleModel>? RoleSet { get; init; }

    /// <summary>
    /// Gets or sets the DbSet for users.
    /// </summary>
    private DbSet<UserModel>? UserSet { get; init; }

    /// <summary>
    /// Gets or sets the DbSet for resources.
    /// </summary>
    private DbSet<ResourceModel>? ResourceSet { get; init; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RoleModel>()
            .HasIndex(it => it.Name)
            .IsUnique();
        modelBuilder.Entity<UserModel>()
            .HasIndex(it => it.Name)
            .IsUnique();
        modelBuilder.Entity<ResourceModel>()
            .HasIndex(it => it.Identifier)
            .IsUnique();
    }

    /// <summary>
    /// Gets the DbSet for roles.
    /// </summary>
    internal DbSet<RoleModel> Roles => RoleSet!;

    /// <summary>
    /// Gets the DbSet for users.
    /// </summary>
    internal DbSet<UserModel> Users => UserSet!;

    /// <summary>
    /// Gets the DbSet for resources.
    /// </summary>
    internal DbSet<ResourceModel> Resources => ResourceSet!;
}
