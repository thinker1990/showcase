using Abstractions.Extensions;
using Infrastructure.ImageProcessing.Graphics;

namespace Infrastructure.ImageProcessing;

/// <summary>
/// Provides methods to interpret annotations and convert them to visual elements.
/// </summary>
internal static class AnnotationInterpreter
{
    /// <summary>
    /// Interprets the given annotation JSON string and converts it to a collection of visual elements.
    /// </summary>
    /// <param name="annotationJson">The JSON string representing the annotations.</param>
    /// <returns>A collection of visual elements.</returns>
    internal static IEnumerable<VisualElement> Interpret(string annotationJson) =>
        annotationJson.Deserialize<IEnumerable<Annotation>>()
            .OrderByDescending(it => it.LayerRank)
            .Select(Parse);

    /// <summary>
    /// Parses the given annotation and converts it to a visual element.
    /// </summary>
    /// <param name="annotation">The annotation to parse.</param>
    /// <returns>The corresponding visual element.</returns>
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

    /// <summary>
    /// Parses a line annotation and converts it to a <see cref="Line"/> visual element.
    /// </summary>
    /// <param name="annotation">The line annotation to parse.</param>
    /// <returns>The corresponding <see cref="Line"/> visual element.</returns>
    private static Line ParseLine(Annotation annotation)
    {
        var start = new Point(annotation.X1, annotation.Y1);
        var end = new Point(annotation.X2, annotation.Y2);
        var (color, thickness, _) = CommonProperties(annotation);

        return new Line(start, end, color, thickness);
    }

    /// <summary>
    /// Parses a circle annotation and converts it to a <see cref="Circle"/> visual element.
    /// </summary>
    /// <param name="annotation">The circle annotation to parse.</param>
    /// <returns>The corresponding <see cref="Circle"/> visual element.</returns>
    private static Circle ParseCircle(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var radius = annotation.Radius;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Circle(center, radius, color, thickness, fill);
    }

    /// <summary>
    /// Parses a rectangle annotation and converts it to a <see cref="Rectangle"/> visual element.
    /// </summary>
    /// <param name="annotation">The rectangle annotation to parse.</param>
    /// <returns>The corresponding <see cref="Rectangle"/> visual element.</returns>
    private static Rectangle ParseRectangle(Annotation annotation)
    {
        var location = new Point(annotation.X, annotation.Y);
        var size = new Size(annotation.Width, annotation.Height);
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Rectangle(location, size, color, thickness, fill);
    }

    /// <summary>
    /// Parses a rotated rectangle annotation and converts it to a <see cref="RotatedRectangle"/> visual element.
    /// </summary>
    /// <param name="annotation">The rotated rectangle annotation to parse.</param>
    /// <returns>The corresponding <see cref="RotatedRectangle"/> visual element.</returns>
    private static RotatedRectangle ParseRotatedRectangle(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var size = new Size(annotation.Width, annotation.Height);
        var rotation = annotation.Rotation;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new RotatedRectangle(center, size, rotation, color, thickness, fill);
    }

    /// <summary>
    /// Parses an ellipse annotation and converts it to an <see cref="Ellipse"/> visual element.
    /// </summary>
    /// <param name="annotation">The ellipse annotation to parse.</param>
    /// <returns>The corresponding <see cref="Ellipse"/> visual element.</returns>
    private static Ellipse ParseEllipse(Annotation annotation)
    {
        var center = FromCoordinate(annotation.Centre);
        var size = new Size(annotation.Width, annotation.Height);
        var rotation = annotation.Rotation;
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Ellipse(center, size, rotation, color, thickness, fill);
    }

    /// <summary>
    /// Parses a polygon annotation and converts it to a <see cref="Polygon"/> visual element.
    /// </summary>
    /// <param name="annotation">The polygon annotation to parse.</param>
    /// <returns>The corresponding <see cref="Polygon"/> visual element.</returns>
    private static Polygon ParsePolygon(Annotation annotation)
    {
        var points = annotation.Points.Select(FromCoordinate);
        var (color, thickness, fill) = CommonProperties(annotation);

        return new Polygon(points, color, thickness, fill);
    }

    /// <summary>
    /// Parses a text annotation and converts it to a <see cref="Text"/> visual element.
    /// </summary>
    /// <param name="annotation">The text annotation to parse.</param>
    /// <returns>The corresponding <see cref="Text"/> visual element.</returns>
    private static Text ParseText(Annotation annotation)
    {
        var location = new Point(annotation.X, annotation.Y);
        var content = annotation.Text;
        var (color, _, _) = CommonProperties(annotation);

        return new Text(location, content, color);
    }

    /// <summary>
    /// Extracts common properties from the given annotation.
    /// </summary>
    /// <param name="annotation">The annotation to extract properties from.</param>
    /// <returns>A tuple containing the color, thickness, and fill color.</returns>
    private static (Color, int, Color?) CommonProperties(Annotation annotation)
    {
        var thickness = annotation.LineWidth;
        var color = FromRgb(annotation.Colour);
        var fill = FromRgb(annotation.FillColour);

        return (color!, thickness, fill);
    }

    /// <summary>
    /// Converts a coordinate array to a <see cref="Point"/>.
    /// </summary>
    /// <param name="pair">The coordinate array.</param>
    /// <returns>The corresponding <see cref="Point"/>.</returns>
    private static Point FromCoordinate(float[] pair) => new(pair[0], pair[1]);

    /// <summary>
    /// Converts an RGB array to a <see cref="Color"/>.
    /// </summary>
    /// <param name="rgb">The RGB array.</param>
    /// <returns>The corresponding <see cref="Color"/>.</returns>
    private static Color? FromRgb(int[] rgb) => rgb.Length switch
    {
        3 => Color.FromRgb(rgb[2], rgb[1], rgb[0]),
        4 => Color.FromRgb(rgb[2], rgb[1], rgb[0]).WithAlpha(rgb[3]),
        _ => null
    };
}
