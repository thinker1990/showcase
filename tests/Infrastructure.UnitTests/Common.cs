using Abstractions.Extensions;
using System.Runtime.InteropServices;

namespace Infrastructure.UnitTests;

public static class Common
{
    public static DirectoryInfo ImageSource => new("images".InBaseDirectory());

    public static FileInfo FileRelativeToBase(params string[] paths) =>
        new(Path.Combine(paths).InBaseDirectory());

    public static FileInfo RandomTemporaryFile() => new(RandomTemporaryPath());

    public static DirectoryInfo RandomTemporaryDirectory() => new(RandomTemporaryPath());

    public static Image EmptyImage() => new([], 800, 800, PixelFormat.Mono8);

    public static async Task<Image> SomeImage()
    {
        using var image = await SomeImageMatrix();
        var pixels = new byte[image.Total() * image.ElemSize()];
        Marshal.Copy(image.Data, pixels, 0, pixels.Length);

        return new Image(pixels, image.Width, image.Height, PixelFormat.Mono8);
    }

    public static async Task<Mat> SomeImageMatrix()
    {
        var file = ImageSource.EnumerateFiles("*.tiff").First();
        var image = await file.AllBytes();

        return ConvertToMat(image);
    }

    public static Mat ConvertToMat(byte[] image) =>
        Mat.FromImageData(image, ImreadModes.Unchanged);

    private static string RandomTemporaryPath()
    {
        var temporaryDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "SiriusCore.UnitTests"));
        temporaryDirectory.Create();

        var randomPath = Utility.UniqueId();
        return randomPath.InDirectory(temporaryDirectory);
    }
}