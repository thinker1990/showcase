using Abstractions.Extensions;
using LanguageExt;
using System.Globalization;
using System.Reactive.Linq;
using static Infrastructure.Activation.SystemTimeValidateResult;

namespace Infrastructure.Activation;

public sealed class SystemTimeTamperProof : IDisposable
{
    private readonly FileInfo _timeRecorder;
    private readonly IEncryptionProvider _encryption;
    private readonly IDisposable _validationSubscription;

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

    public IObservable<SystemTimeValidateResult> ValidationStream { get; }

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

    private IObservable<SystemTimeValidateResult> ValidateAt(TimeSpan interval) =>
        Observable.Interval(interval).SelectMany(_ => CurrentTimeIsAfterLastRecordTime());

    private async Task<SystemTimeValidateResult> CurrentTimeIsAfterLastRecordTime()
    {
        if (!_timeRecorder.Exists()) return RecordLost;

        return (await LatestTimeRecord()).Match(
            time => time >= DateTime.UtcNow ? Hijacked : Normal,
            _ => Invalid);
    }

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

    private IDisposable UpdateLatestTimeIfValidationPass(IObservable<SystemTimeValidateResult> validations) =>
        validations.Where(it => it is Normal)
            .SelectMany(_ => UpdateLatestTime())
            .Subscribe();

    private async Task<Unit> UpdateLatestTime()
    {
        var now = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        await _encryption.Encrypt(now).WriteTo(_timeRecorder);

        return Unit.Default;
    }

    public void Dispose() => _validationSubscription.Dispose();
}