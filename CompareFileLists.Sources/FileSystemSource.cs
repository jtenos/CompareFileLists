namespace CompareFileLists.Sources;

public class FileSystemSource
    : SourceBase
{
    public override string Description => "File System";

    public string DirectoryFullName { get; set; } = default!;

    public override async IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync()
    {
        foreach (FileInfo file in new DirectoryInfo(DirectoryFullName).EnumerateFiles("*", SearchOption.AllDirectories))
        {
            yield return new(FullName: file.FullName, NumBytes: file.Length);
        }
        await Task.CompletedTask;
    }
}
