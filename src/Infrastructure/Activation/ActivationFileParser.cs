using Abstractions.Extensions;

namespace Infrastructure.Activation;

/// <summary>
/// Parses the activation file and provides methods to retrieve its content and signature.
/// </summary>
internal sealed class ActivationFileParser(FileInfo file)
{
    private readonly FileInfo _file = file ?? throw new ArgumentNullException(nameof(file));

    /// <summary>
    /// Gets the content of the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content of the activation file.</returns>
    internal async Task<string> Content()
    {
        var (content, _) = await ContentAndSignaturePair();
        return content.DecodeToUtf8();
    }

    /// <summary>
    /// Gets the signature of the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the signature of the activation file.</returns>
    internal async Task<byte[]> Signature()
    {
        var (_, signature) = await ContentAndSignaturePair();
        return signature.Decode();
    }

    /// <summary>
    /// Gets the trial validity period from the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the trial validity period.</returns>
    internal async Task<ValidityPeriod> TrialValidityPeriod()
    {
        var (start, end) = await DateRangeAtIndex(0, 1);
        return new ValidityPeriod(start, end);
    }

    /// <summary>
    /// Gets the activation validity period from the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the activation validity period.</returns>
    internal async Task<ValidityPeriod> ActivationValidityPeriod()
    {
        var (start, end) = await DateRangeAtIndex(1, 2);
        return new ValidityPeriod(start, end);
    }

    /// <summary>
    /// Gets the machine fingerprint from the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the machine fingerprint.</returns>
    /// <exception cref="IncorrectFileFormatException">Thrown when the fingerprint is not in base64 format.</exception>
    internal async Task<string> MachineFingerprint()
    {
        var fingerprint = await ContentSegmentAt(0);
        if (!fingerprint.IsBase64())
        {
            throw new IncorrectFileFormatException();
        }

        return fingerprint;
    }

    /// <summary>
    /// Gets the date range at the specified indices from the activation file.
    /// </summary>
    /// <param name="startIndex">The start index of the date range.</param>
    /// <param name="endIndex">The end index of the date range.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the date range.</returns>
    /// <exception cref="IncorrectFileFormatException">Thrown when the date format is incorrect.</exception>
    private async Task<(DateOnly, DateOnly)> DateRangeAtIndex(uint startIndex, uint endIndex)
    {
        var start = await ContentSegmentAt(startIndex);
        var end = await ContentSegmentAt(endIndex);

        return (ToDate(start), ToDate(end));
    }

    /// <summary>
    /// Gets the content segment at the specified index from the activation file.
    /// </summary>
    /// <param name="index">The index of the content segment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content segment.</returns>
    /// <exception cref="IncorrectFileFormatException">Thrown when the index is out of range.</exception>
    private async Task<string> ContentSegmentAt(uint index)
    {
        var segments = await ContentSegments();
        if (segments.Length <= index)
        {
            throw new IncorrectFileFormatException();
        }

        return segments[index];
    }

    /// <summary>
    /// Converts the specified string to a <see cref="DateOnly"/> object.
    /// </summary>
    /// <param name="date">The date string to convert.</param>
    /// <returns>The <see cref="DateOnly"/> object.</returns>
    /// <exception cref="IncorrectFileFormatException">Thrown when the date format is incorrect.</exception>
    private static DateOnly ToDate(string date)
    {
        if (!DateOnly.TryParse(date, out var result))
        {
            throw new IncorrectFileFormatException();
        }

        return result;
    }

    /// <summary>
    /// Gets the content segments from the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content segments.</returns>
    private async Task<string[]> ContentSegments() =>
        Split(await Content(), '|');

    /// <summary>
    /// Gets the content and signature pair from the activation file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content and signature pair.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the activation file does not exist.</exception>
    /// <exception cref="IncorrectFileFormatException">Thrown when the file format is incorrect.</exception>
    private async Task<(string, string)> ContentAndSignaturePair()
    {
        if (!_file.Exists())
        {
            throw new FileNotFoundException(nameof(_file));
        }

        var rawText = await _file.AllText();
        var segments = Split(rawText, '.');
        if (segments.Length != 2)
        {
            throw new IncorrectFileFormatException();
        }

        return (segments[0], segments[1]);
    }

    /// <summary>
    /// Splits the specified input string by the specified separator.
    /// </summary>
    /// <param name="input">The input string to split.</param>
    /// <param name="separator">The separator to split the input string by.</param>
    /// <returns>An array of strings that are the result of splitting the input string.</returns>
    private static string[] Split(string input, char separator) =>
        input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
}
