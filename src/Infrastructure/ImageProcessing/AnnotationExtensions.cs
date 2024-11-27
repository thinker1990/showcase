using Abstractions;
using Microsoft.Maui.Graphics.Skia;
using static Infrastructure.ImageProcessing.AnnotationInterpreter;

namespace Infrastructure.ImageProcessing;

/// <summary>
/// Provides extension methods for annotating images.
/// </summary>
public static class AnnotationExtensions
{
    /// <summary>
    /// Annotates the specified image with the given annotations.
    /// </summary>
    /// <param name="annotations">The annotations to apply to the image.</param>
    /// <param name="image">The image to annotate.</param>
    /// <returns>A byte array representing the annotated image in JPEG format.</returns>
    public static byte[] Annotate(this string annotations, Image image)
    {
        using var context = new SkiaBitmapExportContext(image.Width, image.Height, 1.0f);

        context.Canvas.Draw(image);
        context.Canvas.Draw(annotations);

        return context.Image.ToCompressedJpeg();
    }

    /// <summary>
    /// Draws the specified image on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the image on.</param>
    /// <param name="image">The image to draw.</param>
    private static void Draw(this ICanvas canvas, Image image)
    {
        using var rgbImage = image.ToRgb();
        using var target = SkiaImage.FromStream(rgbImage.ToMemoryStream());
        canvas.DrawImage(target, 0, 0, image.Width, image.Height);
    }

    /// <summary>
    /// Draws the specified annotations on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the annotations on.</param>
    /// <param name="annotations">The annotations to draw.</param>
    private static void Draw(this ICanvas canvas, string annotations)
    {
        foreach (var visualElement in Interpret(annotations))
        {
            visualElement.Draw(canvas);
        }
    }

    /// <summary>
    /// Encode the specified image to a compressed JPEG image.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>A byte array representing the compressed JPEG image.</returns>
    private static byte[] ToCompressedJpeg(this IImage image) =>
        image.AsBytes(ImageFormat.Jpeg, 0.75f);
}
