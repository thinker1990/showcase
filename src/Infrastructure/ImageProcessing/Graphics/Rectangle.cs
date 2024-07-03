namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Rectangle : VisualElement
{
    private Bound Bound { get; }

    internal Rectangle(Point location, Size size,
        Color color, int thickness, Color? fillColor = null)
        : base(color, thickness, fillColor)
    {
        Bound = new Bound(location, size);
    }

    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawRectangle(Bound);
        new Cross(Bound.Center, Color).Draw(canvas);
    }

    protected override void Fill(ICanvas canvas) =>
        canvas.FillRectangle(Bound);
}