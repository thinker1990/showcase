using static System.IO.Path;

namespace Abstractions.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DirectoryInfo"/>.
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Determines whether the specified directory exists.
    /// </summary>
    /// <param name="directory">The directory to check.</param>
    /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
    public static bool Exists(this DirectoryInfo directory) =>
        Directory.Exists(directory.FullName);

    /// <summary>
    /// Gets the full path of the specified directory.
    /// </summary>
    /// <param name="directory">The directory to get the path of.</param>
    /// <returns>The full path of the directory.</returns>
    public static string Path(this DirectoryInfo directory) =>
        directory.FullName;

    /// <summary>
    /// Combines the specified path with the base directory of the current application domain.
    /// </summary>
    /// <param name="path">The path to combine.</param>
    /// <returns>The combined path.</returns>
    public static string InBaseDirectory(this string path) =>
        Combine(AppDomain.CurrentDomain.BaseDirectory, path);

    /// <summary>
    /// Combines the specified path with the path of the specified directory.
    /// </summary>
    /// <param name="path">The path to combine.</param>
    /// <param name="directory">The directory to combine the path with.</param>
    /// <returns>The combined path.</returns>
    public static string InDirectory(this string path, DirectoryInfo directory) =>
        Combine(directory.FullName, path);
}
