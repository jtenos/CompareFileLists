using CompareFileLists.AzureBlob;
using CompareFileLists.Core;
using CompareFileLists.FileSystem;
using MergeSortFile;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

FileListComparison flc = new(logger: null, new LineSorter(logger: null));
FileSystemSource source1 = new(@"C:\temp\dir1");
//FileSystemSource source2 = new(@"C:\temp\dir2");
Console.Write("Connection String: ");
string connectionString = Console.ReadLine() ?? "";
Console.Write("Container Name: ");
string containerName = Console.ReadLine() ?? "";
AzureBlobSource source2 = new(connectionString, containerName);

await foreach (Difference diff in flc.CompareSourcesAsync(source1, source2))
{
    Console.WriteLine(diff);
}
