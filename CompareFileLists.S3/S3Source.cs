using CompareFileLists.Core;

namespace CompareFileLists.S3;

public class S3Source
    : SourceBase
{
    protected override IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync()
    {
        throw new NotImplementedException();
    }
}
