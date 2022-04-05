using System.Text.Json;

namespace CompareFileLists.Sources;

public record struct ObjectInfo(string FullName, long NumBytes)
{
    public override string ToString() => JsonSerializer.Serialize(this);
}
