using MergeSortFile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CompareFileLists.Core;

public class FileListComparison
{
    private static readonly DirectoryInfo _tempPath;
    private readonly ILogger<FileListComparison> _logger;
    private readonly LineSorter _lineSorter;

    public FileListComparison(ILogger<FileListComparison>? logger, LineSorter lineSorter)
    {
        _logger = logger ?? new NullLogger<FileListComparison>();
        _lineSorter = lineSorter;
    }

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

            _logger.LogInformation("Writing objects to {file1}", file1.FullName);
            using (StreamWriter fs1 = file1.CreateText())
            {
                await source1.WriteObjectsAsync(fs1);
            }

            _logger.LogInformation("Writing objects to {file2}", file2.FullName);
            using (StreamWriter fs2 = file2.CreateText())
            {
                await source2.WriteObjectsAsync(fs2);
            }

            _logger.LogInformation("Sorting {file1}", file1.FullName);
            _lineSorter.SortFile(
                inputFileFullName: file1.FullName,
                outputFileFullName: file1Sorted.FullName,
                newLine: Environment.NewLine,
                numLinesPerTempFile: 50_000
            );

            _logger.LogInformation("Sorting {file2}", file2.FullName);
            _lineSorter.SortFile(
                inputFileFullName: file2.FullName,
                outputFileFullName: file2Sorted.FullName,
                newLine: Environment.NewLine,
                numLinesPerTempFile: 50_000
            );

            _logger.LogInformation("Comparing files");
            using StreamReader reader1 = file1Sorted.OpenText();
            using StreamReader reader2 = file2Sorted.OpenText();
            (string? line1, string? line2) = (reader1.ReadLine(), reader2.ReadLine());
            while (line1 is not null || line2 is not null)
            {
                ObjectInfo? object1 = line1 is not null ? ObjectInfo.FromFormattedOutput(line1) : null;
                ObjectInfo? object2 = line2 is not null ? ObjectInfo.FromFormattedOutput(line2) : null;

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
            _logger.LogInformation("Deleting files");
            try { file1?.Delete(); } catch { }
            try { file2?.Delete(); } catch { }
            try { file1Sorted?.Delete(); } catch { }
            try { file2Sorted?.Delete(); } catch { }
        }
    }
}
