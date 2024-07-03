namespace Infrastructure.Activation;

public sealed class ValidityPeriod
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    internal ValidityPeriod(DateOnly start, DateOnly end)
    {
        EnsureDateRangeValid(start, end);
        (Start, End) = (start, end);
    }

    internal bool Expired() => !WithinPeriod(Today);

    private bool WithinPeriod(DateOnly date) => Start <= date && date <= End;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    private static void EnsureDateRangeValid(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            throw new ArgumentException($"Start date {start} should before end date {end}.");
        }
    }
}