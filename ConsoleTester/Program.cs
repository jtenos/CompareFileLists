using CompareFileLists.AzureBlob;
using CompareFileLists.Backblaze;
using CompareFileLists.Core;
using CompareFileLists.FileSystem;
using MergeSortFile;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

FileListComparison flc = new(logger: null, new LineSorter(logger: null));

Console.Write("Azure Blob Connection String: ");
string connectionString = Console.ReadLine() ?? "";
Console.Write("Azure Blob Container Name: ");
string containerName = Console.ReadLine() ?? "";
AzureBlobSource source1 = new(connectionString, containerName);

Console.Write("Backblaze Application Key ID: ");
string appKeyID = Console.ReadLine() ?? "";
Console.Write("Backblaze Application Key: ");
string appKey = Console.ReadLine() ?? "";
Console.Write("Backblaze Bucket ID: ");
string bucketID = Console.ReadLine() ?? "";
BackblazeSource source2 = new(appKeyID, appKey, bucketID);

await foreach (Difference diff in flc.CompareSourcesAsync(source1, source2))
{
    Console.WriteLine(diff);
}
