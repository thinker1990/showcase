namespace Abstractions.Extensions;

public static class FileExtensions
{
    public static bool Exists(this FileInfo file) =>
        File.Exists(file.Path());

    public static string Path(this FileInfo file) =>
        file.FullName;

    public static Task<string> AllText(this FileInfo file) =>
        File.ReadAllTextAsync(file.Path());

    public static Task<byte[]> AllBytes(this FileInfo file) =>
        File.ReadAllBytesAsync(file.Path());

    public static Task WriteTo(this string content, FileInfo file) =>
        File.WriteAllTextAsync(file.Path(), content);
}