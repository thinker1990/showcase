using Abstractions.Messaging;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Infrastructure.Messaging;

public sealed class RxBroker : IBroker, IDisposable
{
    private readonly Subject<Datagram> _queue = new();

    public void Publish(string topic, string message)
    {
        EnsureNotEmpty(topic, nameof(topic));
        EnsureNotEmpty(message, nameof(message));

        _queue.OnNext(new Datagram(topic, message));
    }

    public IObservable<string> Subscribe(string topic)
    {
        EnsureNotEmpty(topic, nameof(topic));

        return _queue.Where(it => it.Topic.Equals(topic)).Select(it => it.Message);
    }

    public void Dispose() => _queue.Dispose();
}