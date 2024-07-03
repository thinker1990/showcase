using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

public sealed class AuthorizationContext(DbContextOptions<AuthorizationContext> options) : DbContext(options)
{
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    private DbSet<RoleModel>? RoleSet { get; init; }

    private DbSet<UserModel>? UserSet { get; init; }

    private DbSet<ResourceModel>? ResourceSet { get; init; }

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

    internal DbSet<RoleModel> Roles => RoleSet!;

    internal DbSet<UserModel> Users => UserSet!;

    internal DbSet<ResourceModel> Resources => ResourceSet!;
}