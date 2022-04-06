using System.Text.Json;

namespace CompareFileLists.Core;

public record struct ObjectInfo(string RelativeName, long NumBytes)
{
    public override string ToString() => $"{ RelativeName } | {NumBytes:#,##0}";
}
