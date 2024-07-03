using Abstractions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Infrastructure.Configuration;

public sealed class ConfigurationRepository(IDbContextFactory<ConfigurationContext> contextFactory) : IConfigurationManager
{
    public async Task<string> Get(string key)
    {
        await using var context = CreateContext();
        var settings = await GetSettingsOrThrow(context, key);

        return settings.Value;
    }

    public async Task<IReadOnlyDictionary<string, string>> Get()
    {
        await using var context = CreateContext();
        var settings = await context.Settings.ToListAsync();

        return settings.ToImmutableDictionary(
            it => it.Key,
            it => it.Value);
    }

    public async Task<bool> Add(KeyValuePair<string, string> settings)
    {
        EnsureNotEmpty(settings.Key, "configuration key");
        EnsureNotEmpty(settings.Value, "configuration value");

        await using var context = CreateContext();
        await EnsureNoDuplication(context, settings.Key);

        context.Settings.Add(new Settings(settings.Key, settings.Value));
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(KeyValuePair<string, string> settings)
    {
        await using var context = CreateContext();
        var target = await GetSettingsOrThrow(context, settings.Key);

        target.Value = settings.Value;
        context.Settings.Update(target);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(string key)
    {
        await using var context = CreateContext();
        var target = await GetSettingsOrThrow(context, key);

        context.Settings.Remove(target);
        return await context.SaveChangesAsync() > 0;
    }

    private static async Task EnsureNoDuplication(ConfigurationContext context, string key)
    {
        if (await context.Settings.AnyAsync(it => it.Key == key))
        {
            throw new EntityDuplicateException(key);
        }
    }

    private static async Task<Settings> GetSettingsOrThrow(ConfigurationContext context, string key)
    {
        return await context.Settings.SingleOrDefaultAsync(it => it.Key == key)
               ?? throw new EntityNotFoundException(key);
    }

    private ConfigurationContext CreateContext() => contextFactory.CreateDbContext();
}