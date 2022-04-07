using System.Text.Json.Serialization;

namespace CompareFileLists.Backblaze;

public class FileResult
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = default!;

    [JsonPropertyName("contentLength")]
    public long Size { get; set; }
}
