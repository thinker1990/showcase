namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents a text visual element with a location, content, and color.
/// </summary>
/// <param name="Location">The location of the text.</param>
/// <param name="Content">The content of the text.</param>
/// <param name="Color">The color of the text.</param>
internal sealed record Text(
    Point Location,
    string Content,
    Color Color) : VisualElement(Color)
{
    /// <summary>
    /// Gets the font size of the text.
    /// </summary>
    private static int FontSize => 20;

    /// <summary>
    /// Gets the font of the text.
    /// </summary>
    private static Font Font => new("Microsoft YaHei");

    /// <summary>
    /// Draws the border of the text on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.FontColor = Color;
        canvas.FontSize = FontSize;
        canvas.Font = Font;
        canvas.DrawString(Content, Location.X, Location.Y, HorizontalAlignment.Left);
    }
}
