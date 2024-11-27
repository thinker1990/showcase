namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a line visual element with start and end points, color, and thickness.
/// </summary>
/// <param name="Start">The start point of the line.</param>
/// <param name="End">The end point of the line.</param>
/// <param name="Color">The color of the line.</param>
/// <param name="Thickness">The thickness of the line.</param>
internal sealed record Line(
    Point Start,
    Point End,
    Color Color,
    int Thickness) : VisualElement(Color, Thickness)
{
    /// <summary>
    /// Draws the border of the line on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas) =>
        canvas.DrawLine(Start, End);
}
