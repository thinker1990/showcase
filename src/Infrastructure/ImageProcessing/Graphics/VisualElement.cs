namespace Infrastructure.ImageProcessing.Graphics;

/// <summary>
/// Represents an abstract visual element with color, thickness, and optional fill color.
/// </summary>
/// <param name="Color">The color of the visual element's border.</param>
/// <param name="Thickness">The thickness of the visual element's border.</param>
/// <param name="FillColor">The optional fill color of the visual element.</param>
internal abstract record VisualElement(
    Color Color,
    int Thickness = 1,
    Color? FillColor = null)
{
    /// <summary>
    /// Draws the visual element on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the visual element on.</param>
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

    /// <summary>
    /// Draws the border of the visual element on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw the border on.</param>
    protected abstract void DrawBorder(ICanvas canvas);

    /// <summary>
    /// Fills the visual element on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to fill the visual element on.</param>
    /// <exception cref="InvalidOperationException">Thrown if the fill operation is not supported.</exception>
    protected virtual void Fill(ICanvas canvas) =>
        throw new InvalidOperationException();
}
