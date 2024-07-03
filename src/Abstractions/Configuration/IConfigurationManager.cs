namespace Abstractions.Configuration;

public interface IConfigurationManager
{
    Task<string> Get(string key);

    Task<bool> Add(KeyValuePair<string, string> settings);

    Task<bool> Update(KeyValuePair<string, string> settings);

    Task<bool> Delete(string key);
}