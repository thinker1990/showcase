namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Ellipse : VisualElement
{
    private Bound Bound { get; }

    private float Rotation { get; }

    internal Ellipse(Point center, Size size, float rotation,
        Color color, int thickness, Color? fillColor = null)
        : base(color, thickness, fillColor)
    {
        var (width, height) = size;
        var upperX = center.X - width / 2;
        var upperY = center.Y - height / 2;
        Bound = new Bound(upperX, upperY, width, height);
        Rotation = rotation;
    }

    protected override void DrawBorder(ICanvas canvas)
    {
        Rotate(canvas, () => canvas.DrawEllipse(Bound));
        new Cross(Bound.Center, Color).Draw(canvas);
    }

    protected override void Fill(ICanvas canvas) =>
        Rotate(canvas, () => canvas.FillEllipse(Bound));

    private void Rotate(ICanvas canvas, Action draw)
    {
        canvas.SaveState();
        var (centerX, centerY) = Bound.Center;
        canvas.Rotate(Rotation, centerX, centerY);
        draw();
        canvas.RestoreState();
    }
}