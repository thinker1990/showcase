namespace Abstractions.Messaging;

/// <summary>
/// Defines the contract for a message broker.
/// </summary>
public interface IBroker
{
    /// <summary>
    /// Publishes a message to the specified topic.
    /// </summary>
    /// <param name="topic">The topic to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    void Publish(string topic, string message);

    /// <summary>
    /// Subscribes to the specified topic and returns an observable sequence of messages.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <returns>An observable sequence of messages from the specified topic.</returns>
    IObservable<string> Subscribe(string topic);
}
