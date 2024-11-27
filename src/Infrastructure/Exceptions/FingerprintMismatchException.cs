namespace Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a fingerprint mismatch occurs.
/// </summary>
[Serializable]
public sealed class FingerprintMismatchException : Exception;
