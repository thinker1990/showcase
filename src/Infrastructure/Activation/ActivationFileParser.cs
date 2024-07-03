using Abstractions.Extensions;

namespace Infrastructure.Activation;

internal sealed class ActivationFileParser(FileInfo file)
{
    internal async Task<string> Content()
    {
        var (content, _) = await ContentAndSignaturePair();
        return content.DecodeToUtf8();
    }

    internal async Task<byte[]> Signature()
    {
        var (_, signature) = await ContentAndSignaturePair();
        return signature.Decode();
    }

    internal async Task<ValidityPeriod> TrialValidityPeriod()
    {
        var (start, end) = await DateRangeAtIndex(0, 1);
        return new ValidityPeriod(start, end);
    }

    internal async Task<ValidityPeriod> ActivationValidityPeriod()
    {
        var (start, end) = await DateRangeAtIndex(1, 2);
        return new ValidityPeriod(start, end);
    }

    internal async Task<string> MachineFingerprint()
    {
        var fingerprint = await ContentSegmentAt(0);
        if (!fingerprint.IsBase64())
        {
            throw new IncorrectFileFormatException();
        }

        return fingerprint;
    }

    private async Task<(DateOnly, DateOnly)> DateRangeAtIndex(uint startIndex, uint endIndex)
    {
        var start = await ContentSegmentAt(startIndex);
        var end = await ContentSegmentAt(endIndex);

        return (ToDate(start), ToDate(end));
    }

    private async Task<string> ContentSegmentAt(uint index)
    {
        var segments = await ContentSegments();
        if (segments.Length <= index)
        {
            throw new IncorrectFileFormatException();
        }

        return segments[index];
    }

    private static DateOnly ToDate(string date)
    {
        if (!DateOnly.TryParse(date, out var result))
        {
            throw new IncorrectFileFormatException();
        }

        return result;
    }

    private async Task<string[]> ContentSegments() =>
        Split(await Content(), '|');

    private async Task<(string, string)> ContentAndSignaturePair()
    {
        if (!file.Exists())
        {
            throw new FileNotFoundException(nameof(file));
        }

        var rawText = await file.AllText();
        var segments = Split(rawText, '.');
        if (segments.Length != 2)
        {
            throw new IncorrectFileFormatException();
        }

        return (segments[0], segments[1]);
    }

    private static string[] Split(string input, char separator) =>
        input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
}