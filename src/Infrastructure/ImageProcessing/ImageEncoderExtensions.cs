using Abstractions;

namespace Infrastructure.ImageProcessing;

public static class ImageEncoderExtensions
{
    private const string TiffExtension = ".tiff";
    private const double Convert16BitsTo8BitsFactor = 1.0 / 16;

    private static ImageEncodingParam NoCompression => new(ImwriteFlags.TiffCompression, 1);

    public static byte[] ToTiff(this Image image)
    {
        using var matrix = MatrixOf(image);

        return EncodeToUncompressedTiff(matrix);
    }

    public static byte[] ToViewableTiff(this Image image)
    {
        using var converted = NeedConversion(image)
            ? image.ToRgb()
            : MatrixOf(image);

        return EncodeToUncompressedTiff(converted);
    }

    public static Image Decode(byte[] encodedImage, PixelFormat pixelFormat)
    {
        using var matrix = Mat.FromImageData(encodedImage, ImreadModes.Grayscale | ImreadModes.AnyDepth);
        var correctPixelFormat = matrix.Type().PixelFormatCorrection(pixelFormat);

        return matrix.ToImage(correctPixelFormat);
    }

    internal static Mat ToRgb(this Image image)
    {
        using var source = MatrixOf(image);
        var colorConversion = image.PixelFormat.CorrespondingColorConversion();

        return source.Type() == CV_8UC1
            ? source.CvtColor(colorConversion)
            : source.CvtColor(colorConversion).ConvertScaleAbs(Convert16BitsTo8BitsFactor);
    }

    private static Mat MatrixOf(Image image)
    {
        var matType = image.PixelFormat.CorrespondingMatType();
        return Mat.FromPixelData(image.Height, image.Width, matType, image.PixelData);
    }

    private static bool NeedConversion(Image image) =>
        image.PixelFormat is not Mono8;

    private static byte[] EncodeToUncompressedTiff(Mat matrix) =>
        matrix.ImEncode(TiffExtension, NoCompression);
}