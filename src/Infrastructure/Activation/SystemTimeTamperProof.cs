using Abstractions.Extensions;
using LanguageExt;
using System.Globalization;
using System.Reactive.Linq;
using static Infrastructure.Activation.SystemTimeValidateResult;

namespace Infrastructure.Activation;

/// <summary>
/// Provides tamper-proof validation for system time by recording and validating the last known time.
/// </summary>
public sealed class SystemTimeTamperProof : IDisposable
{
    private readonly FileInfo _timeRecorder;
    private readonly IEncryptionProvider _encryption;
    private readonly IDisposable _validationSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTimeTamperProof"/> class.
    /// </summary>
    /// <param name="timeRecorder">The file used to record the last known time.</param>
    /// <param name="validateInterval">The interval at which to validate the system time.</param>
    /// <param name="encryption">The encryption provider used to encrypt and decrypt the time records.</param>
    public SystemTimeTamperProof(
        FileInfo timeRecorder,
        TimeSpan validateInterval,
        IEncryptionProvider encryption)
    {
        _timeRecorder = timeRecorder;
        _encryption = encryption;

        ValidationStream = ValidateAt(validateInterval).Publish().RefCount();
        _validationSubscription = UpdateLatestTimeIfValidationPass(ValidationStream);
    }

    /// <summary>
    /// Gets the validation stream that emits validation results at the specified interval.
    /// </summary>
    public IObservable<SystemTimeValidateResult> ValidationStream { get; }

    /// <summary>
    /// Bootstraps the tamper-proof validation by checking the current time against the last recorded time.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation result.</returns>
    public async Task<SystemTimeValidateResult> Bootstrap()
    {
        if (_timeRecorder.Exists())
        {
            return await CurrentTimeIsAfterLastRecordTime();
        }

        // Create time record file if not exist, also a vulnerability.
        await UpdateLatestTime();
        return Normal;
    }

    /// <summary>
    /// Validates the system time at the specified interval.
    /// </summary>
    /// <param name="interval">The interval at which to validate the system time.</param>
    /// <returns>An observable sequence of validation results.</returns>
    private IObservable<SystemTimeValidateResult> ValidateAt(TimeSpan interval) =>
        Observable.Interval(interval).SelectMany(_ => CurrentTimeIsAfterLastRecordTime());

    /// <summary>
    /// Checks if the current time is after the last recorded time.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation result.</returns>
    private async Task<SystemTimeValidateResult> CurrentTimeIsAfterLastRecordTime()
    {
        if (!_timeRecorder.Exists()) return RecordLost;

        return (await LatestTimeRecord()).Match(
            time => time >= DateTime.UtcNow ? Hijacked : Normal,
            _ => Invalid);
    }

    /// <summary>
    /// Gets the latest time record from the time recorder file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains either the latest time record or an exception.</returns>
    private async Task<Either<Exception, DateTime>> LatestTimeRecord()
    {
        try
        {
            var record = await _timeRecorder.AllText();
            return DateTime.Parse(_encryption.Decrypt(record), CultureInfo.InvariantCulture);
        }
        catch
        {
            return new RecoverableException("Time record not valid.");
        }
    }

    /// <summary>
    /// Updates the latest time if the validation passes.
    /// </summary>
    /// <param name="validations">The observable sequence of validation results.</param>
    /// <returns>A disposable object that unsubscribes from the validation stream when disposed.</returns>
    private IDisposable UpdateLatestTimeIfValidationPass(IObservable<SystemTimeValidateResult> validations) =>
        validations.Where(it => it is Normal)
            .SelectMany(_ => UpdateLatestTime())
            .Subscribe();

    /// <summary>
    /// Updates the latest time in the time recorder file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task<Unit> UpdateLatestTime()
    {
        var now = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        await _encryption.Encrypt(now).WriteTo(_timeRecorder);

        return Unit.Default;
    }

    /// <summary>
    /// Disposes the resources used by the <see cref="SystemTimeTamperProof"/> class.
    /// </summary>
    public void Dispose() => _validationSubscription.Dispose();
}
