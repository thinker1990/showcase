namespace Abstractions.Exceptions;

[Serializable]
public sealed class EntityNotFoundException(string name) : Exception($"Can not find {name}.");