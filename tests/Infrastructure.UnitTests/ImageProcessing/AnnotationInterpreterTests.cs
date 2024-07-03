using Infrastructure.ImageProcessing;
using Microsoft.Maui.Graphics.Skia;
using static Infrastructure.UnitTests.AnnotationBuilder;

namespace Infrastructure.UnitTests.ImageProcessing;

internal sealed class AnnotationInterpreterTests
{
    [Test]
    public void ShouldParseRawAnnotationsCorrectly()
    {
        var (width, height) = (800, 800);
        var rawAnnotation = From(width, height).Build();

        var annotations = AnnotationInterpreter.Interpret(rawAnnotation);

        annotations.Should().NotBeEmpty();
    }

    [Test]
    [Category(LongRunning)]
    public async Task ShouldDrawAnnotationsCorrectly()
    {
        var (width, height) = (800, 800);
        var annotations = AnnotationInterpreter.Interpret(From(width, height).Build());
        using var context = new SkiaBitmapExportContext(width, height, 1.0f);

        foreach (var annotation in annotations)
        {
            annotation.Draw(context.Canvas);
        }

        var image = await context.Image.AsBytesAsync();
        var display = () => Display(image, nameof(ShouldDrawAnnotationsCorrectly));
        display.Should().NotThrow();
    }
}