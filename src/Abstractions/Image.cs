namespace Abstractions;

public sealed record Image(
    byte[] PixelData,
    int Width,
    int Height,
    PixelFormat PixelFormat)
{
    public string Id { get; } = Utility.UniqueId();
}