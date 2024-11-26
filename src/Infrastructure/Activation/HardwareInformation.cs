using System.Management;

namespace Infrastructure.Activation;

/// <summary>
/// Provides methods to retrieve hardware information for generating a machine fingerprint.
/// </summary>
public static class HardwareInformation
{
    /// <summary>
    /// Generates a machine fingerprint by combining various hardware identifiers.
    /// </summary>
    /// <returns>A Base64 encoded string representing the machine fingerprint.</returns>
    public static string Fingerprint()
    {
        var fingerprint = $"{ProcessorId()}{BiosId()}{DiskId()}{VolumeSerialNumber()}{BaseBoardId()}";
        return fingerprint.EncodeToBase64();
    }

    /// <summary>
    /// Gets the processor ID.
    /// </summary>
    /// <returns>The processor ID.</returns>
    internal static string ProcessorId() =>
        Identifier("Win32_Processor", "ProcessorId");

    /// <summary>
    /// Gets the BIOS serial number.
    /// </summary>
    /// <returns>The BIOS serial number.</returns>
    internal static string BiosId() =>
        Identifier("Win32_BIOS", "SerialNumber");

    /// <summary>
    /// Gets the disk model and signature.
    /// </summary>
    /// <returns>The disk model and signature.</returns>
    internal static string DiskId() =>
        CombinedIdentifier("Win32_DiskDrive", "Model", "Signature");

    /// <summary>
    /// Gets the volume serial number of the C: drive.
    /// </summary>
    /// <returns>The volume serial number of the C: drive.</returns>
    internal static string VolumeSerialNumber()
    {
        using var disk = new ManagementObject("Win32_LogicalDisk.DeviceID=\"C:\"");
        return disk["VolumeSerialNumber"]?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the baseboard model and serial number.
    /// </summary>
    /// <returns>The baseboard model and serial number.</returns>
    internal static string BaseBoardId() =>
        CombinedIdentifier("Win32_BaseBoard", "Model", "SerialNumber");

    /// <summary>
    /// Combines multiple hardware identifiers into a single string.
    /// </summary>
    /// <param name="class">The WMI class to query.</param>
    /// <param name="properties">The properties to retrieve from the WMI class.</param>
    /// <returns>A combined string of the specified properties.</returns>
    private static string CombinedIdentifier(string @class, params string[] properties) =>
        string.Join(string.Empty, properties.Select(it => Identifier(@class, it)));

    /// <summary>
    /// Retrieves a specific hardware identifier.
    /// </summary>
    /// <param name="class">The WMI class to query.</param>
    /// <param name="property">The property to retrieve from the WMI class.</param>
    /// <returns>The value of the specified property.</returns>
    private static string Identifier(string @class, string property)
    {
        using var managementClass = new ManagementClass(@class);
        return managementClass.GetInstances()
            .OfType<ManagementBaseObject>()
            .Select(it => it[property]?.ToString())
            .FirstOrDefault(it => !string.IsNullOrEmpty(it)) ?? string.Empty;
    }
}
