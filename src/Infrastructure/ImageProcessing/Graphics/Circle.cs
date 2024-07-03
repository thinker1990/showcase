namespace Infrastructure.ImageProcessing.Graphics;

internal sealed record Circle(
    Point Center,
    float Radius,
    Color Color,
    int Thickness,
    Color? FillColor = null) : VisualElement(Color, Thickness, FillColor)
{
    protected override void DrawBorder(ICanvas canvas)
    {
        canvas.DrawCircle(Center, Radius);
        new Cross(Center, Color).Draw(canvas);
    }

    protected override void Fill(ICanvas canvas) =>
        canvas.FillCircle(Center, Radius);
}