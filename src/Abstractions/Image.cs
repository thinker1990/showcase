namespace Abstractions;

/// <summary>
/// Represents an image with pixel data, dimensions, and pixel format.
/// </summary>
/// <param name="PixelData">The pixel data of the image.</param>
/// <param name="Width">The width of the image.</param>
/// <param name="Height">The height of the image.</param>
/// <param name="PixelFormat">The pixel format of the image.</param>
public sealed record Image(
    byte[] PixelData,
    int Width,
    int Height,
    PixelFormat PixelFormat)
{
    /// <summary>
    /// Gets the unique identifier of the image.
    /// </summary>
    public string Id { get; } = Utility.UniqueId();
}
