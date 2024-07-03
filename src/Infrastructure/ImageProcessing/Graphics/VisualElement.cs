namespace Infrastructure.ImageProcessing.Graphics;

internal abstract record VisualElement(
    Color Color,
    int Thickness = 1,
    Color? FillColor = null)
{
    internal void Draw(ICanvas canvas)
    {
        if (FillColor is not null)
        {
            canvas.FillColor = FillColor;
            Fill(canvas);
        }

        canvas.StrokeColor = Color;
        canvas.StrokeSize = Thickness;
        DrawBorder(canvas);
    }

    protected abstract void DrawBorder(ICanvas canvas);

    protected virtual void Fill(ICanvas canvas) =>
        throw new InvalidOperationException();
}