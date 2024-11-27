using Abstractions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Infrastructure.Configuration;

/// <summary>
/// Repository for managing configuration settings.
/// </summary>
public sealed class ConfigurationRepository(IDbContextFactory<ConfigurationContext> contextFactory) : IConfigurationManager
{
    /// <summary>
    /// Retrieves the value of a configuration setting by its key.
    /// </summary>
    /// <param name="key">The key of the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the configuration setting.</returns>
    public async Task<string> Get(string key)
    {
        await using var context = CreateContext();
        var settings = await GetSettingsOrThrow(context, key);

        return settings.Value;
    }

    /// <summary>
    /// Retrieves all configuration settings.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only dictionary of all configuration settings.</returns>
    public async Task<IReadOnlyDictionary<string, string>> Get()
    {
        await using var context = CreateContext();
        var settings = await context.Settings.ToListAsync();

        return settings.ToImmutableDictionary(
            it => it.Key,
            it => it.Value);
    }

    /// <summary>
    /// Adds a new configuration setting.
    /// </summary>
    /// <param name="settings">The key-value pair representing the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    public async Task<bool> Add(KeyValuePair<string, string> settings)
    {
        EnsureNotEmpty(settings.Key, "configuration key");
        EnsureNotEmpty(settings.Value, "configuration value");

        await using var context = CreateContext();
        await EnsureNoDuplication(context, settings.Key);

        context.Settings.Add(new Settings(settings.Key, settings.Value));
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Updates an existing configuration setting.
    /// </summary>
    /// <param name="settings">The key-value pair representing the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    public async Task<bool> Update(KeyValuePair<string, string> settings)
    {
        await using var context = CreateContext();
        var target = await GetSettingsOrThrow(context, settings.Key);

        target.Value = settings.Value;
        context.Settings.Update(target);
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Deletes a configuration setting by its key.
    /// </summary>
    /// <param name="key">The key of the configuration setting to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    public async Task<bool> Delete(string key)
    {
        await using var context = CreateContext();
        var target = await GetSettingsOrThrow(context, key);

        context.Settings.Remove(target);
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Ensures that there is no duplication of the configuration key.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="key">The key to check for duplication.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="EntityDuplicateException">Thrown when a duplicate key is found.</exception>
    private static async Task EnsureNoDuplication(ConfigurationContext context, string key)
    {
        if (await context.Settings.AnyAsync(it => it.Key == key))
        {
            throw new EntityDuplicateException(key);
        }
    }

    /// <summary>
    /// Retrieves the configuration setting by its key or throws an exception if not found.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="key">The key of the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the configuration setting.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the key is not found.</exception>
    private static async Task<Settings> GetSettingsOrThrow(ConfigurationContext context, string key)
    {
        return await context.Settings.SingleOrDefaultAsync(it => it.Key == key)
               ?? throw new EntityNotFoundException(key);
    }

    /// <summary>
    /// Creates a new instance of the configuration context.
    /// </summary>
    /// <returns>A new instance of the configuration context.</returns>
    private ConfigurationContext CreateContext() => contextFactory.CreateDbContext();
}
