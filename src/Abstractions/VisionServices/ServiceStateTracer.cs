using System.Reactive.Subjects;

namespace Abstractions.VisionServices;

/// <summary>
/// Traces the state of a service.
/// </summary>
/// <param name="initial">The initial state of the service.</param>
public sealed class ServiceStateTracer(string initial) : StateTracer<string>
{
    /// <summary>
    /// Gets the state of the service.
    /// </summary>
    protected override BehaviorSubject<string> State { get; } = new(Capitalize(initial));

    /// <summary>
    /// Updates the state of the service.
    /// </summary>
    /// <param name="newState">The new state of the service.</param>
    public void Update(string newState) => ChangeTo(Capitalize(newState));

    /// <summary>
    /// Capitalizes the first letter of the input string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input string with the first letter capitalized.</returns>
    private static string Capitalize(string input) =>
        string.IsNullOrEmpty(input) ? input : $"{char.ToUpper(input[0])}{input[1..]}";
}
