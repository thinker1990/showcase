namespace Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when an operation is attempted on an expired entity.
/// </summary>
[Serializable]
public sealed class ExpiredException : Exception;
