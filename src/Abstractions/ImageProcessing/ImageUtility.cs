// ReSharper disable UnusedParameter.Local

namespace Abstractions.ImageProcessing;

public static class ImageUtility
{
    public static Func<Image, byte[]> ToTiff { get; set; } =
        image => throw SetupBeforeUse();

    public static Func<byte[], PixelFormat, Image> DecodeImage { get; set; } =
        (encodedImage, pixelFormat) => throw SetupBeforeUse();

    public static Func<string, Image, byte[]> AnnotateImage { get; set; } =
        (annotations, image) => throw SetupBeforeUse();

    private static Exception SetupBeforeUse() =>
        new InvalidOperationException("Please setup before use.");
}