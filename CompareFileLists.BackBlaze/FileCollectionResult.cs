using System.Text.Json.Serialization;

namespace CompareFileLists.Backblaze;

public class FileCollectionResult
{
    [JsonPropertyName("files")]
    public FileResult[] Files { get; set; } = default!;

    [JsonPropertyName("nextFileName")]
    public string? NextFileName { get; set; }

}
