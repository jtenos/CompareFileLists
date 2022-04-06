using System.Text.Json;

namespace CompareFileLists.Core;

public class FileListComparison
{
    private static readonly DirectoryInfo _tempPath;

    static FileListComparison()
    {
        _tempPath = new(Path.Combine(Path.GetTempPath(), "FileListComparison"));
        _tempPath.Create();
    }

    public async IAsyncEnumerable<Difference> CompareSourcesAsync(SourceBase source1, SourceBase source2)
    {
        FileInfo? file1 = null;
        FileInfo? file2 = null;
        FileInfo? file1Sorted = null;
        FileInfo? file2Sorted = null;
        try
        {
            file1 = new(Path.Combine(_tempPath.FullName, Guid.NewGuid().ToString("N")));
            file1Sorted = new($"{file1.FullName}.sorted");
            file2 = new(Path.Combine(_tempPath.FullName, Guid.NewGuid().ToString("N")));
            file2Sorted = new($"{file2.FullName}.sorted");

            using (StreamWriter fs1 = file1.CreateText())
            {
                await source1.WriteObjectsAsJsonAsync(fs1);
            }
            using (StreamWriter fs2 = file2.CreateText())
            {
                await source2.WriteObjectsAsJsonAsync(fs2);
            }

            await SortFileAsync(file1, file1Sorted).ConfigureAwait(false);
            await SortFileAsync(file2, file2Sorted).ConfigureAwait(false);

            using StreamReader reader1 = file1.OpenText();
            using StreamReader reader2 = file2.OpenText();
            (string? line1, string? line2) = (reader1.ReadLine(), reader2.ReadLine());
            while (line1 is not null || line2 is not null)
            {
                ObjectInfo? object1 = line1 is not null ? JsonSerializer.Deserialize<ObjectInfo>(line1) : null;
                ObjectInfo? object2 = line2 is not null ? JsonSerializer.Deserialize<ObjectInfo>(line2) : null;

                if (!object1.HasValue)
                {
                    yield return new Difference(object1, object2, DifferenceType.MissingFromSource1);
                    line2 = reader2.ReadLine();
                    continue;
                }
                
                if (!object2.HasValue)
                {
                    yield return new Difference(object1, object2, DifferenceType.MissingFromSource2);
                    line1 = reader1.ReadLine();
                    continue;
                }
                
                if (string.Compare(object1.Value.RelativeName, object2.Value.RelativeName) < 0)
                {
                    yield return new Difference(object1, null, DifferenceType.MissingFromSource2);
                    line1 = reader1.ReadLine();
                    continue;
                }

                if (string.Compare(object1.Value.RelativeName, object2.Value.RelativeName) > 0)
                {
                    yield return new Difference(null, object2, DifferenceType.MissingFromSource1);
                    line2 = reader2.ReadLine();
                    continue;
                }

                if (object1.Value.NumBytes != object2.Value.NumBytes)
                {
                    yield return new Difference(object1, object2, DifferenceType.NumBytesDifferent);
                    (line1, line2) = (reader1.ReadLine(), reader2.ReadLine());
                    continue;
                }

                (line1, line2) = (reader1.ReadLine(), reader2.ReadLine());
            }
        }
        finally
        {
            try { file1?.Delete(); } catch { }
            try { file2?.Delete(); } catch { }
            try { file1Sorted?.Delete(); } catch { }
            try { file2Sorted?.Delete(); } catch { }
        }
    }

    private static async Task SortFileAsync(FileInfo unsortedFile, FileInfo sortedFile)
    {
        IEnumerable<ObjectInfo> sortedObjects = (await File.ReadAllLinesAsync(unsortedFile.FullName).ConfigureAwait(false))
            .Select(line => JsonSerializer.Deserialize<ObjectInfo>(line))
            .OrderBy(oi => oi.RelativeName);

        await File.WriteAllLinesAsync(sortedFile.FullName, sortedObjects.Select(oi => JsonSerializer.Serialize(oi)));
    }
}
