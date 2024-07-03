namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Line(
    Point Start,
    Point End,
    Color Color,
    int Thickness) : VisualElement(Color, Thickness)
{
    protected override void DrawBorder(ICanvas canvas) =>
        canvas.DrawLine(Start, End);
}