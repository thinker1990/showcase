namespace Infrastructure.Activation;

/// <summary>
/// Represents the result of system time validation.
/// </summary>
public enum SystemTimeValidateResult
{
    /// <summary>
    /// The system time is valid and normal.
    /// </summary>
    Normal,

    /// <summary>
    /// The time record is lost.
    /// </summary>
    RecordLost,

    /// <summary>
    /// The time record is invalid.
    /// </summary>
    Invalid,

    /// <summary>
    /// The system time has been tampered with.
    /// </summary>
    Hijacked
}
