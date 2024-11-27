using Abstractions.Messaging;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Infrastructure.Messaging;

/// <summary>
/// Represents a reactive message broker.
/// </summary>
public sealed class RxBroker : IBroker, IDisposable
{
    private readonly Subject<Datagram> _queue = new();

    /// <summary>
    /// Publishes a message to the specified topic.
    /// </summary>
    /// <param name="topic">The topic to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    public void Publish(string topic, string message)
    {
        EnsureNotEmpty(topic, nameof(topic));
        EnsureNotEmpty(message, nameof(message));

        _queue.OnNext(new Datagram(topic, message));
    }

    /// <summary>
    /// Subscribes to the specified topic and returns an observable sequence of messages.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <returns>An observable sequence of messages from the specified topic.</returns>
    public IObservable<string> Subscribe(string topic)
    {
        EnsureNotEmpty(topic, nameof(topic));

        return _queue.Where(it => it.Topic.Equals(topic)).Select(it => it.Message);
    }

    /// <summary>
    /// Disposes the resources used by the RxBroker.
    /// </summary>
    public void Dispose() => _queue.Dispose();
}
