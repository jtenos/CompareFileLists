using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CompareFileLists.Core;

namespace CompareFileLists.AzureBlob;

public class AzureBlobSource
    : SourceBase
{
    public AzureBlobSource(string connectionString, string containerName)
    {
        ConnectionString = connectionString;
        ContainerName = containerName;
    }

    private string ConnectionString { get; }
    private string ContainerName { get; }

    protected override async IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync()
    {
        BlobContainerClient containerClient = new(ConnectionString, ContainerName);

        foreach (BlobItem blob in containerClient.GetBlobs())
        {
            yield return new(blob.Name, blob.Properties.ContentLength ?? -1);
        }
        await Task.CompletedTask;
    }
}
