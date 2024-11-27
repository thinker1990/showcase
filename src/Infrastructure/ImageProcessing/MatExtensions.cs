using Abstractions;
using static System.Runtime.InteropServices.Marshal;

namespace Infrastructure.ImageProcessing;

/// <summary>
/// Provides extension methods for working with Mat objects.
/// </summary>
internal static class MatExtensions
{
    /// <summary>
    /// Gets the corresponding MatType for the specified PixelFormat.
    /// </summary>
    /// <param name="pixelFormat">The pixel format.</param>
    /// <returns>The corresponding MatType.</returns>
    /// <exception cref="NotSupportedException">Thrown if the pixel format is not supported.</exception>
    internal static MatType CorrespondingMatType(this PixelFormat pixelFormat) =>
        pixelFormat switch
        {
            Mono8 or BayerRG8 => CV_8UC1,
            Mono12 or BayerRG12 => CV_16UC1,
            _ => throw new NotSupportedException($"{pixelFormat} is not supported.")
        };

    /// <summary>
    /// Corrects the pixel format based on the MatType.
    /// </summary>
    /// <param name="matType">The MatType.</param>
    /// <param name="candidate">The candidate pixel format.</param>
    /// <returns>The corrected pixel format.</returns>
    /// <exception cref="NotSupportedException">Thrown if the MatType is not supported.</exception>
    internal static PixelFormat PixelFormatCorrection(this MatType matType, PixelFormat candidate)
    {
        if (matType == CV_8UC1)
        {
            return candidate is Mono8 or BayerRG8 ? candidate : Mono8;
        }

        if (matType == CV_16UC1)
        {
            return candidate is Mono12 or BayerRG12 ? candidate : Mono12;
        }

        throw new NotSupportedException($"{matType} is not supported.");
    }

    /// <summary>
    /// Gets the corresponding color conversion code for the specified PixelFormat.
    /// </summary>
    /// <param name="pixelFormat">The pixel format.</param>
    /// <returns>The corresponding color conversion code.</returns>
    /// <exception cref="NotSupportedException">Thrown if the pixel format is not supported.</exception>
    internal static ColorConversionCodes CorrespondingColorConversion(this PixelFormat pixelFormat) =>
        pixelFormat switch
        {
            Mono8 or Mono12 => ColorConversionCodes.GRAY2RGB,
            BayerRG8 or BayerRG12 => ColorConversionCodes.BayerRG2RGB,
            _ => throw new NotSupportedException($"{pixelFormat} is not supported.")
        };

    /// <summary>
    /// Converts the specified Mat object to an Image.
    /// </summary>
    /// <param name="image">The Mat object.</param>
    /// <param name="pixelFormat">The pixel format.</param>
    /// <returns>The converted Image.</returns>
    internal static Image ToImage(this Mat image, PixelFormat pixelFormat)
    {
        var pixels = PixelDataOf(image);
        return new Image(pixels, image.Width, image.Height, pixelFormat);
    }

    /// <summary>
    /// Gets the pixel data of the specified Mat object.
    /// </summary>
    /// <param name="image">The Mat object.</param>
    /// <returns>A byte array containing the pixel data.</returns>
    private static byte[] PixelDataOf(Mat image)
    {
        var pixels = new byte[image.Total() * image.ElemSize()];
        Copy(image.Data, pixels, 0, pixels.Length);

        return pixels;
    }
}
