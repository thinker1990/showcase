namespace Abstractions.Authorization;

/// <summary>
/// Specifies the access control modes for an element.
/// </summary>
public enum AccessControlMode
{
    /// <summary>
    /// The element is hidden.
    /// </summary>
    Hidden,

    /// <summary>
    /// The element is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// The element is visible.
    /// </summary>
    Visible
}
