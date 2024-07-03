using System.Management;

namespace Infrastructure.Activation;

public static class HardwareInformation
{
    public static string Fingerprint()
    {
        var fingerprint = $"{ProcessorId()}{BiosId()}{DiskId()}{VolumeSerialNumber()}{BaseBoardId()}";

        return fingerprint.EncodeToBase64();
    }

    internal static string ProcessorId() =>
        Identifier("Win32_Processor", "ProcessorId");

    internal static string BiosId() =>
        Identifier("Win32_BIOS", "SerialNumber");

    internal static string DiskId() =>
        CombinedIdentifier("Win32_DiskDrive", "Model", "Signature");

    internal static string VolumeSerialNumber()
    {
        var disk = new ManagementObject("Win32_LogicalDisk.DeviceID=\"C:\"");
        return $"{disk["VolumeSerialNumber"]}";
    }

    internal static string BaseBoardId() =>
        CombinedIdentifier("Win32_BaseBoard", "Model", "SerialNumber");

    private static string CombinedIdentifier(string @class, params string[] properties) =>
        string.Join(string.Empty, properties.Select(it => Identifier(@class, it)));

    private static string Identifier(string @class, string property) =>
        new ManagementClass(@class).GetInstances()
            .OfType<ManagementBaseObject>()
            .Select(it => $"{it[property]}")
            .FirstOrDefault(it => !string.IsNullOrEmpty(it)) ?? string.Empty;
}