using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration;

/// <summary>
/// Represents the configuration context for the application.
/// </summary>
/// <param name="options">The options to configure the context.</param>
public sealed class ConfigurationContext(DbContextOptions<ConfigurationContext> options) : DbContext(options)
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    /// <summary>
    /// Gets or sets the DbSet for settings.
    /// </summary>
    private DbSet<Settings>? SettingsSet { get; init; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Settings>()
            .HasKey(it => it.Key);
    }

    /// <summary>
    /// Gets the DbSet for settings.
    /// </summary>
    internal DbSet<Settings> Settings => SettingsSet!;
}
