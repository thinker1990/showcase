using System.Reactive.Subjects;

namespace Abstractions.VisionServices;

public sealed class ServiceStateTracer(string initial) : StateTracer<string>
{
    protected override BehaviorSubject<string> State { get; }
        = new(Capitalize(initial));

    public void Update(string newState) =>
        ChangeTo(Capitalize(newState));

    private static string Capitalize(string input) =>
        $"{input[..1].ToUpper()}{input[1..]}";
}