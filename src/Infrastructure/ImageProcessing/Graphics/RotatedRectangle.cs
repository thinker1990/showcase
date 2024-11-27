namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a rotated rectangle visual element with a center point, size, rotation, color, thickness, and optional fill color.
/// </summary>
internal sealed record RotatedRectangle : VisualElement
{
    /// <summary>
    /// Gets the bounding box of the rotated rectangle.
    /// </summary>
    private Bound Bound { get; }

    /// <summary>
    /// Gets the rotation angle of the rectangle.
    /// </summary>
    private float Rotation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RotatedRectangle"/> record.
    /// </summary>
    /// <param name="center">The center point of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="rotation">The rotation angle of the rectangle.</param>
    /// <param name="color">The color of the rectangle's border.</param>
    /// <param name="thickness">The thickness of the rectangle's border.</param>
    /// <param name="fillColor">The optional fill color of the rectangle.</param>
    internal RotatedRectangle(Point center, Size size, float rotation,
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
    /// Draws the border of the rotated rectangle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        Rotate(canvas, () => canvas.DrawRectangle(Bound));
        new Cross(Bound.Center, Color).Draw(canvas);
    }

    /// <summary>
    /// Fills the rotated rectangle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the rectangle on.</param>
    protected override void Fill(ICanvas canvas) =>
        Rotate(canvas, () => canvas.FillRectangle(Bound));

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
