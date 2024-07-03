using Infrastructure.ImageProcessing.Graphics;
using Microsoft.Maui.Graphics.Skia;
using System.Runtime.CompilerServices;
using Point = Microsoft.Maui.Graphics.PointF;
using Size = Microsoft.Maui.Graphics.SizeF;

namespace Infrastructure.UnitTests.ImageProcessing;

[Category(LongRunning)]
internal sealed class DrawingVisualElementsTests
{
    private static Size CanvasSize => new(500, 500);
    private static Point Center => new(CanvasSize.Width / 2, CanvasSize.Height / 2);
    private static float Radius => CanvasSize.Width / 10;
    private static Color Color => Colors.Green;
    private static int Thickness => 1;

    private SkiaBitmapExportContext _context = default!;
    private ICanvas Canvas => _context.Canvas;

    [SetUp]
    public void Setup() => _context = EmptyCanvas();

    [Test]
    public Task ShouldDrawLineCorrectly()
    {
        var (start, end) = (new Point(10, 10), new Point(200, 200));
        var line = new Line(start, end, Color, Thickness);

        line.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawTextCorrectly()
    {
        var (location1, content1) = (new Point(150, 200), "你好，世界");
        var (location2, content2) = (new Point(150, 250), "Hello, World");
        var text1 = new Text(location1, content1, Color);
        var text2 = new Text(location2, content2, Color);

        text1.Draw(Canvas);
        text2.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawCircleBorderCorrectly()
    {
        var circle = new Circle(Center, Radius, Color, Thickness);

        circle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldFillCircleCorrectly()
    {
        var circle = new Circle(Center, Radius, Color, Thickness, Color);

        circle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawRectangleBorderCorrectly()
    {
        var (location, size) = (new Point(200, 200), new Size(100, 100));
        var rectangle = new Rectangle(location, size, Color, Thickness);

        rectangle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldFillRectangleCorrectly()
    {
        var (location, size) = (new Point(200, 200), new Size(100, 100));
        var rectangle = new Rectangle(location, size, Color, Thickness, Color);

        rectangle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawRotatedRectangleBorderCorrectly()
    {
        var (size, angle) = (new Size(100, 100), 45);
        var rectangle = new RotatedRectangle(Center, size, angle, Color, Thickness);

        rectangle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldFillRotatedRectangleCorrectly()
    {
        var (size, angle) = (new Size(100, 100), 45);
        var rectangle = new RotatedRectangle(Center, size, angle, Color, Thickness, Color);

        rectangle.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawEllipseBorderCorrectly()
    {
        var (size, angle) = (new Size(120, 80), 90);
        var ellipse = new Ellipse(Center, size, angle, Color, Thickness);

        ellipse.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldFillEllipseCorrectly()
    {
        var (size, angle) = (new Size(120, 80), 45);
        var ellipse = new Ellipse(Center, size, angle, Color, Thickness, Color);

        ellipse.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldDrawPolygonBorderCorrectly()
    {
        var vertices = new[]
        {
            new Point(50, 50), new Point(300, 50),
            new Point(300, 300), new Point(50, 300)
        };
        var polygon = new Polygon(vertices, Color, Thickness);

        polygon.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public Task ShouldFillPolygonCorrectly()
    {
        var vertices = new[]
        {
            new Point(50, 50), new Point(300, 50),
            new Point(300, 300), new Point(50, 300)
        };
        var polygon = new Polygon(vertices, Color, Thickness, Color);

        polygon.Draw(Canvas);

        return DisplayImage();
    }

    [Test]
    public async Task ShouldDrawImageCorrectly()
    {
        var image = await TestImage();

        Canvas.DrawImage(image, 0, 0, image.Width, image.Height);

        await DisplayImage();
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private async Task DisplayImage([CallerMemberName] string identifier = "")
    {
        var image = await _context.Image.AsBytesAsync();
        var display = () => Display(image, identifier);
        display.Should().NotThrow();
    }

    private static SkiaBitmapExportContext EmptyCanvas()
    {
        var (width, height) = CanvasSize;
        return new SkiaBitmapExportContext((int)width, (int)height, 1.0f);
    }

    private static async Task<IImage> TestImage()
    {
        using var image = await SomeImageMatrix();
        return SkiaImage.FromStream(image.ToMemoryStream());
    }
}