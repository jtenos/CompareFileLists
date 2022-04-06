using CompareFileLists.Core;

namespace CompareFileLists.FileSystem;

public class FileSystemSource
    : SourceBase
{
    public FileSystemSource(string directoryFullName)
    {
        DirectoryFullName = directoryFullName;
    }

    public string DirectoryFullName { get; }

    protected override async IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync()
    {
        DirectoryInfo dir = new(DirectoryFullName);
        IEnumerable<FileInfo> files = dir.EnumerateFiles("*", SearchOption.AllDirectories);

        string path = dir.FullName.Replace("/", "\\");
        if (!path.EndsWith("\\")) { path += "\\"; }
        foreach (FileInfo file in files)
        {
            string relativeFileName = file.FullName[path.Length..]; // Strip off the base directory
            yield return new(RelativeName: relativeFileName, NumBytes: file.Length);
        }
        await Task.CompletedTask;
    }
}
