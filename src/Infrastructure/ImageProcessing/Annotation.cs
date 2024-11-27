namespace Infrastructure.ImageProcessing;

/// <summary>
/// Represents an annotation with various properties such as type, layer rank, dimensions, and text.
/// </summary>
/// <param name="Type">The type of the annotation.</param>
/// <param name="LayerRank">The layer rank of the annotation.</param>
/// <param name="LineWidth">The line width of the annotation.</param>
/// <param name="X">The X coordinate of the annotation.</param>
/// <param name="Y">The Y coordinate of the annotation.</param>
/// <param name="X1">The X1 coordinate of the annotation.</param>
/// <param name="Y1">The Y1 coordinate of the annotation.</param>
/// <param name="X2">The X2 coordinate of the annotation.</param>
/// <param name="Y2">The Y2 coordinate of the annotation.</param>
/// <param name="Width">The width of the annotation.</param>
/// <param name="Height">The height of the annotation.</param>
/// <param name="Centre">The center coordinates of the annotation.</param>
/// <param name="Radius">The radius of the annotation.</param>
/// <param name="Points">The points defining the annotation.</param>
/// <param name="Rotation">The rotation of the annotation.</param>
/// <param name="Text">The text of the annotation.</param>
internal sealed record Annotation(
    AnnotationType Type,
    int LayerRank,
    int LineWidth,
    float X,
    float Y,
    float X1,
    float Y1,
    float X2,
    float Y2,
    float Width,
    float Height,
    float[] Centre,
    float Radius,
    float[][] Points,
    float Rotation,
    string Text = "")
{
    /// <summary>
    /// Gets or initializes the color of the annotation.
    /// </summary>
    public int[] Colour { get; init; } = [];

    /// <summary>
    /// Gets or initializes the fill color of the annotation.
    /// </summary>
    public int[] FillColour { get; init; } = [];
}

/// <summary>
/// Specifies the type of annotation.
/// </summary>
internal enum AnnotationType
{
    Line,
    Circle,
    Rectangle,
    RotatedRectangle,
    Ellipse,
    Polygon,
    Text
}
