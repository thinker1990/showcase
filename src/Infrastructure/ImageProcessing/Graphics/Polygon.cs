namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a polygon visual element with a collection of points, color, thickness, and optional fill color.
/// </summary>
internal sealed record Polygon : VisualElement
{
    /// <summary>
    /// Gets the path of the polygon.
    /// </summary>
    private PathF PolygonPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon"/> record.
    /// </summary>
    /// <param name="points">The points that define the polygon.</param>
    /// <param name="color">The color of the polygon's border.</param>
    /// <param name="thickness">The thickness of the polygon's border.</param>
    /// <param name="fillColor">The optional fill color of the polygon.</param>
    internal Polygon(IEnumerable<Point> points,
        Color color, int thickness, Color? fillColor = null)
        : base(color, thickness, fillColor)
    {
        // ReSharper disable PossibleMultipleEnumeration
        var (head, rest) = (points.First(), points.Skip(1));

        PolygonPath = new PathF(head);
        foreach (var point in rest)
        {
            PolygonPath.LineTo(point);
        }

        PolygonPath.Close();
    }

    /// <summary>
    /// Draws the border of the polygon on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas) =>
        canvas.DrawPath(PolygonPath);

    /// <summary>
    /// Fills the polygon on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the polygon on.</param>
    protected override void Fill(ICanvas canvas) =>
        canvas.FillPath(PolygonPath);
}
