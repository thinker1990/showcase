namespace Abstractions.Messaging;

public interface IBroker
{
    void Publish(string topic, string message);

    IObservable<string> Subscribe(string topic);
}