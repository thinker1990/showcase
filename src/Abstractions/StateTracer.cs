using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Abstractions;

public abstract class StateTracer<TState> : IDisposable
{
    private bool _disposed;

    protected abstract BehaviorSubject<TState> State { get; }

    public TState Current => State.Value;

    public IObservable<TState> Changed => State.AsObservable();

    internal void ChangeTo(TState target) => State.OnNext(target);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            State.Dispose();
        }

        _disposed = true;
    }
}