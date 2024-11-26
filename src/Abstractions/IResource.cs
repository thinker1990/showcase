namespace Abstractions;

/// <summary>
/// Defines the contract for a resource.
/// </summary>
public interface IResource : IDisposable
{
    /// <summary>
    /// Gets the unique identifier of the resource.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the resource.
    /// </summary>
    string Name { get; }
}
