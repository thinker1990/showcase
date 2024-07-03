namespace Abstractions;

public interface IResource : IDisposable
{
    string Id { get; }

    string Name { get; }
}