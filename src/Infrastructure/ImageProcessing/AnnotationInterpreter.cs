using Abstractions.Extensions;
using Infrastructure.ImageProcessing.Graphics;

namespace Infrastructure.ImageProcessing;

internal static class AnnotationInterpreter
{
    internal static IEnumerable<VisualElement> Interpret(string annotationJson) =>
        annotationJson.Deserialize<IEnumerable<Annotation>>()
            .OrderByDescending(it => it.LayerRank)
            .Select(Parse);

    private static VisualElement Parse(Annotation annotation) => annotation.Type switch
    {
        AnnotationType.Line => ParseLine(annotation),
        AnnotationType.Circle => ParseCircle(annotation),
        AnnotationType.Rectangle => ParseRectangle(annotation),
        AnnotationType.RotatedRectangle => ParseRotatedRectangle(annotation),
        AnnotationType.Ellipse => ParseEllipse(annotation),
        AnnotationType.Polygon => ParsePolygon(annotation),
        AnnotationType.Text => ParseText(annotation),
        _ => throw new NotSupportedException($"{annotation.Type} is not supported.")
    };

    private static VisualElement ParseLine(Annotation annotation)
    {
        var start = new Point(annotation.X1, annotation.Y1);
        var end = new Point(annotation.X2, annotation.Y2);
        var (color, thickness, _) = CommonProperties(annotation);

        return new Line(start, end, color, thickness);
    }

    private static VisualElement ParseCircle(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var radius = annotation.Radius;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Circle(center, radius, color, thickness, fill);
    }

    private static VisualElement ParseRectangle(Annotation annotation)
    {
        var location = new Point(annotation.X, annotation.Y);
        var size = new Size(annotation.Width, annotation.Height);
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Rectangle(location, size, color, thickness, fill);
    }

    private static VisualElement ParseRotatedRectangle(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var size = new Size(annotation.Width, annotation.Height);
        var rotation = annotation.Rotation;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new RotatedRectangle(center, size, rotation, color, thickness, fill);
    }

    private static VisualElement ParseEllipse(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var size = new Size(annotation.Width, annotation.Height);
        var rotation = annotation.Rotation;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Ellipse(center, size, rotation, color, thickness, fill);
    }

    private static VisualElement ParsePolygon(Annotation annotation)
    {
        var points = annotation.Points.Select(FromCoordinate);
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Polygon(points, color, thickness, fill);
    }

    private static VisualElement ParseText(Annotation annotation)
    {
        var location = new Point(annotation.X, annotation.Y);
        var content = annotation.Text;
        var (color, _, _) = CommonProperties(annotation);

        return new Text(location, content, color);
    }

    private static (Color, int, Color?) CommonProperties(Annotation annotation)
    {
        var thickness = annotation.LineWidth;
        var color = FromRgb(annotation.Colour);
        var fill = FromRgb(annotation.FillColour);

        return (color!, thickness, fill);
    }

    private static Point FromCoordinate(float[] pair) => new(pair[0], pair[1]);

    private static Color? FromRgb(IReadOnlyList<int> rgb) => rgb.Length() switch
    {
        3 => Color.FromRgb(rgb[2], rgb[1], rgb[0]),
        4 => Color.FromRgb(rgb[2], rgb[1], rgb[0]).WithAlpha(rgb[3]),
        _ => null
    };
}