using Abstractions;
using Microsoft.Maui.Graphics.Skia;
using static Infrastructure.ImageProcessing.AnnotationInterpreter;

namespace Infrastructure.ImageProcessing;

public static class AnnotationExtensions
{
    public static byte[] Annotate(this string annotations, Image image)
    {
        using var context = new SkiaBitmapExportContext(image.Width, image.Height, 1.0f);

        context.Canvas.Draw(image);
        context.Canvas.Draw(annotations);

        return context.Image.ToCompressedJpeg();
    }

    private static void Draw(this ICanvas canvas, Image image)
    {
        using var rgbImage = image.ToRgb();
        using var target = SkiaImage.FromStream(rgbImage.ToMemoryStream());
        canvas.DrawImage(target, 0, 0, image.Width, image.Height);
    }

    private static void Draw(this ICanvas canvas, string annotations)
    {
        foreach (var visualElement in Interpret(annotations))
        {
            visualElement.Draw(canvas);
        }
    }

    private static byte[] ToCompressedJpeg(this IImage image) =>
        image.AsBytes(ImageFormat.Jpeg, 0.75f);
}