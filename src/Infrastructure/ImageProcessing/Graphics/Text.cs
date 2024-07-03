namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Text(
    Point Location,
    string Content,
    Color Color) : VisualElement(Color)
{
    private static int FontSize => 20;

    private static Font Font => new("Microsoft YaHei");

    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.FontColor = Color;
        canvas.FontSize = FontSize;
        canvas.Font = Font;
        canvas.DrawString(Content, Location.X, Location.Y, HorizontalAlignment.Left);
    }
}