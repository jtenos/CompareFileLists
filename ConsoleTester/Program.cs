using CompareFileLists.Core;
using CompareFileLists.FileSystem;

FileListComparison flc = new();
FileSystemSource source1 = new(@"C:\temp\dir1");
FileSystemSource source2 = new(@"C:\temp\dir2");

await foreach (Difference diff in flc.CompareSourcesAsync(source1, source2))
{
    Console.WriteLine(diff);
}
