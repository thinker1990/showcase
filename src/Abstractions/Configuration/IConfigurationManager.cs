namespace Abstractions.Configuration;

/// <summary>
/// Defines the contract for a configuration manager.
/// </summary>
public interface IConfigurationManager
{
    /// <summary>
    /// Retrieves the value of a configuration setting by its key.
    /// </summary>
    /// <param name="key">The key of the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the configuration setting.</returns>
    Task<string> Get(string key);

    /// <summary>
    /// Adds a new configuration setting.
    /// </summary>
    /// <param name="settings">The key-value pair representing the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    Task<bool> Add(KeyValuePair<string, string> settings);

    /// <summary>
    /// Updates an existing configuration setting.
    /// </summary>
    /// <param name="settings">The key-value pair representing the configuration setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    Task<bool> Update(KeyValuePair<string, string> settings);

    /// <summary>
    /// Deletes a configuration setting by its key.
    /// </summary>
    /// <param name="key">The key of the configuration setting to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    Task<bool> Delete(string key);
}
