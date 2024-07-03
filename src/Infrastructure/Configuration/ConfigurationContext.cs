using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration;

public sealed class ConfigurationContext(DbContextOptions<ConfigurationContext> options) : DbContext(options)
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private DbSet<Settings>? SettingsSet { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Settings>()
            .HasKey(it => it.Key);
    }

    internal DbSet<Settings> Settings => SettingsSet!;
}