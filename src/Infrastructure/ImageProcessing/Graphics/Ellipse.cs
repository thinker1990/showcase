namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents an ellipse visual element with a bounding box, rotation, color, thickness, and optional fill color.
/// </summary>
internal sealed record Ellipse : VisualElement
{
    /// <summary>
    /// Gets the bounding box of the ellipse.
    /// </summary>
    private Bound Bound { get; }

    /// <summary>
    /// Gets the rotation angle of the ellipse.
    /// </summary>
    private float Rotation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> record.
    /// </summary>
    /// <param name="center">The center point of the ellipse.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="rotation">The rotation angle of the ellipse.</param>
    /// <param name="color">The color of the ellipse's border.</param>
    /// <param name="thickness">The thickness of the ellipse's border.</param>
    /// <param name="fillColor">The optional fill color of the ellipse.</param>
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

    /// <summary>
    /// Draws the border of the ellipse on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        Rotate(canvas, () => canvas.DrawEllipse(Bound));
        new Cross(Bound.Center, Color).Draw(canvas);
    }

    /// <summary>
    /// Fills the ellipse on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the ellipse on.</param>
    protected override void Fill(ICanvas canvas) =>
        Rotate(canvas, () => canvas.FillEllipse(Bound));

    /// <summary>
    /// Rotates the canvas and performs the specified drawing action.
    /// </summary>
    /// <param name="canvas">The canvas to rotate and draw on.</param>
    /// <param name="draw">The drawing action to perform.</param>
    private void Rotate(ICanvas canvas, Action draw)
    {
        canvas.SaveState();
        var (centerX, centerY) = Bound.Center;
        canvas.Rotate(Rotation, centerX, centerY);
        draw();
        canvas.RestoreState();
    }
}
