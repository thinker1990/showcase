using static System.IO.Path;

namespace Abstractions.Extensions;

public static class DirectoryExtensions
{
    public static bool Exists(this DirectoryInfo directory) =>
        Directory.Exists(directory.Path());

    public static string Path(this DirectoryInfo directory) =>
        directory.FullName;

    public static string InBaseDirectory(this string path) =>
        Combine(AppDomain.CurrentDomain.BaseDirectory, path);

    public static string InDirectory(this string path, DirectoryInfo directory) =>
        Combine(directory.Path(), path);
}