using Abstractions;
using static System.Runtime.InteropServices.Marshal;

namespace Infrastructure.ImageProcessing;

internal static class MatExtensions
{
    internal static MatType CorrespondingMatType(this PixelFormat pixelFormat) =>
        pixelFormat switch
        {
            Mono8 or BayerRG8 => CV_8UC1,
            Mono12 or BayerRG12 => CV_16UC1,
            _ => throw new NotSupportedException($"{pixelFormat} is not supported.")
        };

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

    internal static ColorConversionCodes CorrespondingColorConversion(this PixelFormat pixelFormat) =>
        pixelFormat switch
        {
            Mono8 or Mono12 => ColorConversionCodes.GRAY2RGB,
            BayerRG8 or BayerRG12 => ColorConversionCodes.BayerRG2RGB,
            _ => throw new NotSupportedException($"{pixelFormat} is not supported.")
        };

    internal static Image ToImage(this Mat image, PixelFormat pixelFormat)
    {
        var pixels = PixelDataOf(image);
        return new Image(pixels, image.Width, image.Height, pixelFormat);
    }

    private static byte[] PixelDataOf(Mat image)
    {
        var pixels = new byte[image.Total() * image.ElemSize()];
        Copy(image.Data, pixels, 0, pixels.Length);

        return pixels;
    }
}