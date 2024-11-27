namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a rectangle visual element with a location, size, color, thickness, and optional fill color.
/// </summary>
internal sealed record Rectangle : VisualElement
{
    /// <summary>
    /// Gets the bounding box of the rectangle.
    /// </summary>
    private Bound Bound { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rectangle"/> record.
    /// </summary>
    /// <param name="location">The location of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle's border.</param>
    /// <param name="thickness">The thickness of the rectangle's border.</param>
    /// <param name="fillColor">The optional fill color of the rectangle.</param>
    internal Rectangle(Point location, Size size,
        Color color, int thickness, Color? fillColor = null)
        : base(color, thickness, fillColor)
    {
        Bound = new Bound(location, size);
    }

    /// <summary>
    /// Draws the border of the rectangle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawRectangle(Bound);
        new Cross(Bound.Center, Color).Draw(canvas);
    }

    /// <summary>
    /// Fills the rectangle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the rectangle on.</param>
    protected override void Fill(ICanvas canvas) =>
        canvas.FillRectangle(Bound);
}
