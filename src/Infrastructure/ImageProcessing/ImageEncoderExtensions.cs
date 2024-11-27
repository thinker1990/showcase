using Abstractions;

namespace Infrastructure.ImageProcessing;

/// <summary>
/// Provides extension methods for encoding and decoding images.
/// </summary>
public static class ImageEncoderExtensions
{
    private const string TiffExtension = ".tiff";
    private const double Convert16BitsTo8BitsFactor = 1.0 / 16;

    private static ImageEncodingParam NoCompression => new(ImwriteFlags.TiffCompression, 1);

    /// <summary>
    /// Encode the specified image to an uncompressed TIFF byte array.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>A byte array representing the uncompressed TIFF image.</returns>
    public static byte[] ToTiff(this Image image)
    {
        using var matrix = MatrixOf(image);

        return EncodeToUncompressedTiff(matrix);
    }

    /// <summary>
    /// Encode the specified image to a viewable uncompressed TIFF byte array.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>A byte array representing the viewable uncompressed TIFF image.</returns>
    public static byte[] ToViewableTiff(this Image image)
    {
        using var converted = NeedConversion(image)
            ? image.ToRgb()
            : MatrixOf(image);

        return EncodeToUncompressedTiff(converted);
    }

    /// <summary>
    /// Decodes the specified byte array to an image.
    /// </summary>
    /// <param name="encodedImage">The byte array representing the encoded image.</param>
    /// <param name="pixelFormat">The pixel format of the image.</param>
    /// <returns>The decoded image.</returns>
    public static Image Decode(byte[] encodedImage, PixelFormat pixelFormat)
    {
        using var matrix = Mat.FromImageData(encodedImage, ImreadModes.Grayscale | ImreadModes.AnyDepth);
        var correctPixelFormat = matrix.Type().PixelFormatCorrection(pixelFormat);

        return matrix.ToImage(correctPixelFormat);
    }

    /// <summary>
    /// Convert the specified image to an RGB matrix.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>The RGB matrix.</returns>
    internal static Mat ToRgb(this Image image)
    {
        using var source = MatrixOf(image);
        var colorConversion = image.PixelFormat.CorrespondingColorConversion();

        return source.Type() == CV_8UC1
            ? source.CvtColor(colorConversion)
            : source.CvtColor(colorConversion).ConvertScaleAbs(Convert16BitsTo8BitsFactor);
    }

    /// <summary>
    /// Converts the specified image to a matrix.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>The matrix representing the image.</returns>
    private static Mat MatrixOf(Image image)
    {
        var matType = image.PixelFormat.CorrespondingMatType();
        return Mat.FromPixelData(image.Height, image.Width, matType, image.PixelData);
    }

    /// <summary>
    /// Determines whether the specified image needs color conversion.
    /// </summary>
    /// <param name="image">The image to check.</param>
    /// <returns><c>true</c> if the image needs conversion; otherwise, <c>false</c>.</returns>
    private static bool NeedConversion(Image image) =>
        image.PixelFormat is not Mono8;

    /// <summary>
    /// Encodes the specified matrix to an uncompressed TIFF byte array.
    /// </summary>
    /// <param name="matrix">The matrix to encode.</param>
    /// <returns>A byte array representing the uncompressed TIFF image.</returns>
    private static byte[] EncodeToUncompressedTiff(Mat matrix) =>
        matrix.ImEncode(TiffExtension, NoCompression);
}
