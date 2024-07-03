namespace Abstractions.Exceptions;

[Serializable]
public class RecoverableException : Exception
{
    public RecoverableException(string message)
        : base(message)
    {
    }

    public RecoverableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}