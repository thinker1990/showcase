namespace Infrastructure.UnitTests;

public sealed class AnnotationBuilder
{
    private const int Thickness = 2;
    private const int Rotation = 75;

    private int CenterX { get; }

    private int CenterY { get; }

    private int Width { get; }

    private int Height { get; }

    private int Radius { get; }

    private AnnotationBuilder(int width, int height)
    {
        (CenterX, CenterY) = (width / 2, height / 2);
        (Width, Height) = (width / 3, width * 2 / 3);
        Radius = width / 3;
    }

    public static AnnotationBuilder From(Image image) => From(image.Width, image.Height);

    public static AnnotationBuilder From(int width, int height) => new(width, height);

    public string Build() =>
        $$"""
          [
              {
                  "Type": "Circle",
                  "LayerName": "Blister Edge",
                  "LayerRank": 10,
                  "Colour": [255, 0, 0],
                  "LineWidth": {{Thickness}},
                  "Centre": [{{CenterX}}, {{CenterY}}],
                  "Radius": {{Radius}},
                  "Name": "Blister Edge",
                  "Tag": "Dividing line"
              },
              {
                  "Type": "Ellipse",
                  "LayerName": "Optical Zone",
                  "LayerRank": 20,
                  "Colour": [0, 128, 255],
                  "LineWidth": {{Thickness}},
                  "Centre": [{{CenterX}}, {{CenterY}}],
                  "Width": {{Width}},
                  "Height": {{Height}},
                  "Rotation": {{Rotation}},
                  "Name": "Optical Zone",
                  "Tag": "Dividing line"
              },
              {
                  "Type": "RotatedRectangle",
                  "LayerName": "Foreign Material",
                  "LayerRank": 30,
                  "Colour": [0, 255, 0],
                  "LineWidth": {{Thickness}},
                  "Centre": [{{CenterX}}, {{CenterY}}],
                  "Width": {{Width}},
                  "Height": {{Height}},
                  "Rotation": {{-Rotation}},
                  "Name": "Foreign Material",
                  "Tag": "Zone1"
              },
              {
                  "Type": "Polygon",
                  "LayerName": "Scratch",
                  "LayerRank": 40,
                  "Colour": [241, 158, 230],
                  "LineWidth": {{Thickness}},
                  "FillColour": [202, 168, 173, 255],
                  "Points": [
                      [{{CenterX}}, {{CenterY}}],
                      [{{CenterX - 100}}, {{CenterY + 400}}],
                      [{{CenterX + 100}}, {{CenterY + 400}}]
                  ]
              },
              {
                  "Type": "Text",
                  "Text": "{{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}}",
                  "LayerName": "Im text",
                  "LayerRank": 50,
                  "Colour": [241, 158, 230],
                  "X": 50,
                  "Y": 50
              },
              {
                  "Type": "Line",
                  "Colour": [200, 200, 200],
                  "LineWidth": {{Thickness}},
                  "X1": 50,
                  "Y1": 100,
                  "X2": 600,
                  "Y2": 100,
                  "LayerName": "Im line",
                  "LayerRank": 60
              }
          ]
          """;
}