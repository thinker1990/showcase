namespace Abstractions.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan Seconds(this int seconds) =>
        TimeSpan.FromSeconds(seconds);

    public static TimeSpan Milliseconds(this int milliseconds) =>
        TimeSpan.FromMilliseconds(milliseconds);
}