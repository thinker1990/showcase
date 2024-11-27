namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a circle visual element with a center point, radius, color, thickness, and optional fill color.
/// </summary>
/// <param name="Center">The center point of the circle.</param>
/// <param name="Radius">The radius of the circle.</param>
/// <param name="Color">The color of the circle's border.</param>
/// <param name="Thickness">The thickness of the circle's border.</param>
/// <param name="FillColor">The optional fill color of the circle.</param>
internal sealed record Circle(
    Point Center,
    float Radius,
    Color Color,
    int Thickness,
    Color? FillColor = null) : VisualElement(Color, Thickness, FillColor)
{
    /// <summary>
    /// Draws the border of the circle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawCircle(Center, Radius);
        new Cross(Center, Color).Draw(canvas);
    }

    /// <summary>
    /// Fills the circle on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the circle on.</param>
    protected override void Fill(ICanvas canvas) =>
        canvas.FillCircle(Center, Radius);
}
