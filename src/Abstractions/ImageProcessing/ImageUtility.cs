// ReSharper disable UnusedParameter.Local

namespace Abstractions.ImageProcessing;

/// <summary>
/// Provides utility methods for image processing.
/// </summary>
public static class ImageUtility
{
    /// <summary>
    /// Gets or sets the function to encode an image to TIFF format.
    /// </summary>
    public static Func<Image, byte[]> ToTiff { get; set; } =
        image => throw SetupBeforeUse();

    /// <summary>
    /// Gets or sets the function to decode an TIFF image.
    /// </summary>
    public static Func<byte[], PixelFormat, Image> DecodeImage { get; set; } =
        (encodedImage, pixelFormat) => throw SetupBeforeUse();

    /// <summary>
    /// Gets or sets the function to annotate an image.
    /// </summary>
    public static Func<string, Image, byte[]> AnnotateImage { get; set; } =
        (annotations, image) => throw SetupBeforeUse();

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> indicating that the utility must be set up before use.
    /// </summary>
    /// <returns>An <see cref="InvalidOperationException"/>.</returns>
    private static InvalidOperationException SetupBeforeUse() => new("Please setup before use.");
}
