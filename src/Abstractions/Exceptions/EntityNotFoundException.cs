namespace Abstractions.Exceptions;

/// <summary>
/// The exception that is thrown when an entity cannot be found.
/// </summary>
[Serializable]
public sealed class EntityNotFoundException(string name) : Exception($"Can not find {name}.");
