namespace Abstractions.Exceptions;

[Serializable]
public sealed class EntityDuplicateException(string name) : Exception($"{name} already exists.");