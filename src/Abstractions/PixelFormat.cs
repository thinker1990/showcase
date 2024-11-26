// ReSharper disable InconsistentNaming

namespace Abstractions;

/// <summary>
/// Specifies the pixel format of an image.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 8-bit monochrome pixel format.
    /// </summary>
    Mono8,

    /// <summary>
    /// 12-bit monochrome pixel format.
    /// </summary>
    Mono12,

    /// <summary>
    /// 8-bit Bayer RG pixel format.
    /// </summary>
    BayerRG8,

    /// <summary>
    /// 12-bit Bayer RG pixel format.
    /// </summary>
    BayerRG12
}
