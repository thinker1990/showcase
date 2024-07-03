namespace Abstractions.VisionServices;

public interface IVisionServer
{
    string Host { get; }

    Task<IEnumerable<IVisionService>> Services { get; }
}