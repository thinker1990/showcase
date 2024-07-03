namespace Infrastructure.ImageProcessing;

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
    public int[] Colour { get; init; } = [];

    public int[] FillColour { get; init; } = [];
}

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