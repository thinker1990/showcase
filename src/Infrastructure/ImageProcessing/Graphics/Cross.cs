namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a cross visual element with a center point and color.
/// </summary>
/// <param name="Center">The center point of the cross.</param>
/// <param name="Color">The color of the cross.</param>
internal sealed record Cross(
    Point Center,
    Color Color) : VisualElement(Color, Thickness: 1)
{
    private static float Size => 4.0f;

    /// <summary>
    /// Draws the border of the cross on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawLine(Center.X - Size, Center.Y, Center.X + Size, Center.Y);
        canvas.DrawLine(Center.X, Center.Y - Size, Center.X, Center.Y + Size);
    }
}
