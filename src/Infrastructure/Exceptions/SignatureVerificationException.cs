namespace Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when signature verification fails.
/// </summary>
[Serializable]
public sealed class SignatureVerificationException : Exception;
