namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Cross(
    Point Center,
    Color Color) : VisualElement(Color, Thickness: 1)
{
    private static float Size => 4.0f;

    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawLine(Center.X - Size, Center.Y, Center.X + Size, Center.Y);
        canvas.DrawLine(Center.X, Center.Y - Size, Center.X, Center.Y + Size);
    }
}