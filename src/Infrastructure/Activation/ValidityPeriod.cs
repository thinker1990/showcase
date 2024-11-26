namespace Infrastructure.Activation;

/// <summary>
/// Represents a validity period with a start and end date.
/// </summary>
public sealed class ValidityPeriod
{
    /// <summary>
    /// Gets the start date of the validity period.
    /// </summary>
    public DateOnly Start { get; }

    /// <summary>
    /// Gets the end date of the validity period.
    /// </summary>
    public DateOnly End { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidityPeriod"/> class with the specified start and end dates.
    /// </summary>
    /// <param name="start">The start date of the validity period.</param>
    /// <param name="end">The end date of the validity period.</param>
    /// <exception cref="ArgumentException">Thrown when the start date is after the end date.</exception>
    internal ValidityPeriod(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            throw new ArgumentException($"Start date {start} should be before end date {end}.");
        }

        (Start, End) = (start, end);
    }

    /// <summary>
    /// Determines whether the validity period has expired.
    /// </summary>
    /// <returns><c>true</c> if the validity period has expired; otherwise, <c>false</c>.</returns>
    internal bool Expired() => !WithinPeriod(DateOnly.FromDateTime(DateTime.Today));

    /// <summary>
    /// Determines whether the specified date is within the validity period.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> if the date is within the validity period; otherwise, <c>false</c>.</returns>
    private bool WithinPeriod(DateOnly date) => Start <= date && date <= End;
}
