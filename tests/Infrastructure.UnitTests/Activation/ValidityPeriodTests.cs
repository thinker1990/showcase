namespace Infrastructure.UnitTests.Activation;

internal sealed class ValidityPeriodTests
{
    [Test]
    public void StartAndEndDateShouldBeTheSameAsSpecified()
    {
        var (start, end) = (LastMonth, NextMonth);

        var period = new ValidityPeriod(start, end);

        period.Start.Should().Be(start);
        period.End.Should().Be(end);
    }

    [Test]
    public void ShouldNotExpireWhenDateWithinValidityPeriod()
    {
        var (start, end) = (LastMonth, NextMonth);

        var period = new ValidityPeriod(start, end);

        period.Expired().Should().BeFalse();
    }

    [Test]
    public void ShouldNotExpireWhenDateEqualsStartDateOfValidityPeriod()
    {
        var (start, end) = (Today, NextMonth);

        var period = new ValidityPeriod(start, end);

        period.Expired().Should().BeFalse();
    }

    [Test]
    public void ShouldExpiredWhenDateBeforeStartDateOfValidityPeriod()
    {
        var (start, end) = (Tomorrow, NextMonth);

        var period = new ValidityPeriod(start, end);

        period.Expired().Should().BeTrue();
    }

    [Test]
    public void ShouldNotExpireWhenDateEqualsEndDateOfValidityPeriod()
    {
        var (start, end) = (LastMonth, Today);

        var period = new ValidityPeriod(start, end);

        period.Expired().Should().BeFalse();
    }

    [Test]
    public void ShouldExpiredWhenDateAfterEndDateOfValidityPeriod()
    {
        var (start, end) = (LastMonth, Yesterday);

        var period = new ValidityPeriod(start, end);

        period.Expired().Should().BeTrue();
    }

    [Test]
    public void ShouldThrowExceptionWhenStartDateNotBeforeEndDate()
    {
        var (start, end) = (NextMonth, LastMonth);

        var initialize = () => new ValidityPeriod(start, end);

        initialize.Should().Throw<ArgumentException>();
    }

    private static DateOnly Yesterday => Today.AddDays(-1);

    private static DateOnly Tomorrow => Today.AddDays(1);

    private static DateOnly LastMonth => Today.AddMonths(-1);

    private static DateOnly NextMonth => Today.AddMonths(1);

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}