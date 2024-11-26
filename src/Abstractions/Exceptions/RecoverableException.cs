namespace Abstractions.Exceptions;

/// <summary>
/// Represents an exception that can be recovered from.
/// </summary>
[Serializable]
public class RecoverableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecoverableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RecoverableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecoverableException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RecoverableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
