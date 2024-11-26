namespace Abstractions.Exceptions;

/// <summary>
/// The exception that is thrown when an entity with the same name already exists.
/// </summary>
[Serializable]
public sealed class EntityDuplicateException(string name) : Exception($"{name} already exists.");
