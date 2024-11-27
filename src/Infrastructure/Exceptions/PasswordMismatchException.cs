namespace Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a password mismatch occurs.
/// </summary>
[Serializable]
public sealed class PasswordMismatchException : Exception;
