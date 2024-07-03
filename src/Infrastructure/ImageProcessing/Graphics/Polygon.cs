namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Polygon : VisualElement
{
    private PathF PolygonPath { get; }

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

    protected override void DrawBorder(ICanvas canvas) =>
        canvas.DrawPath(PolygonPath);

    protected override void Fill(ICanvas canvas) =>
        canvas.FillPath(PolygonPath);
}