namespace Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when an incorrect file format is encountered.
/// </summary>
[Serializable]
public sealed class IncorrectFileFormatException : Exception;
