using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Abstractions;

/// <summary>
/// Provides a base class for tracing the state of an object.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
public abstract class StateTracer<TState> : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the state subject.
    /// </summary>
    protected abstract BehaviorSubject<TState> State { get; }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    public TState Current => State.Value;

    /// <summary>
    /// Gets an observable sequence of state changes.
    /// </summary>
    public IObservable<TState> Changed => State.AsObservable();

    /// <summary>
    /// Changes the state to the specified target state.
    /// </summary>
    /// <param name="target">The target state.</param>
    internal void ChangeTo(TState target) => State.OnNext(target);

    /// <summary>
    /// Disposes the resources used by the <see cref="StateTracer{TState}"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources used by the <see cref="StateTracer{TState}"/>.
    /// </summary>
    /// <param name="disposing">A value indicating whether the method is called from the <see cref="Dispose"/> method.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            State?.Dispose();
        }

        _disposed = true;
    }
}
