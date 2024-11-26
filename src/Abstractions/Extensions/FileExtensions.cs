namespace Abstractions.Extensions;

/// <summary>
/// Provides extension methods for <see cref="FileInfo"/>.
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    public static bool Exists(this FileInfo file) =>
        File.Exists(file.FullName);

    /// <summary>
    /// Gets the full path of the specified file.
    /// </summary>
    /// <param name="file">The file to get the path of.</param>
    /// <returns>The full path of the file.</returns>
    public static string Path(this FileInfo file) =>
        file.FullName;

    /// <summary>
    /// Reads all text from the specified file asynchronously.
    /// </summary>
    /// <param name="file">The file to read from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the text read from the file.</returns>
    public static Task<string> AllText(this FileInfo file) =>
        File.ReadAllTextAsync(file.FullName);

    /// <summary>
    /// Reads all bytes from the specified file asynchronously.
    /// </summary>
    /// <param name="file">The file to read from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the bytes read from the file.</returns>
    public static Task<byte[]> AllBytes(this FileInfo file) =>
        File.ReadAllBytesAsync(file.FullName);

    /// <summary>
    /// Writes the specified content to the file asynchronously.
    /// </summary>
    /// <param name="content">The content to write to the file.</param>
    /// <param name="file">The file to write to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task WriteTo(this string content, FileInfo file) =>
        File.WriteAllTextAsync(file.FullName, content);
}
