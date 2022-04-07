namespace CompareFileLists.Core;

public record struct ObjectInfo(string RelativeName, long NumBytes)
{
    public override string ToString() => $"{RelativeName} | {NumBytes:#,##0}";

    public string ToFormattedOutput() => $"{RelativeName}{NumBytes:000000000000}";
    public static ObjectInfo FromFormattedOutput(string formatted)
    {
        string relativeName = formatted[0..^12];
        long numBytes = long.Parse(formatted[^12..]);
        return new(relativeName, numBytes);
    }
}
