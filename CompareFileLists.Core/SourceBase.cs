using System.Text;
using System.Text.Json;

namespace CompareFileLists.Core;

public abstract class SourceBase
{
    /// <summary>
    /// Enumerates over the objects (files), in no particular order. The caller
    /// will need to sort the results prior to comparing them.
    /// </summary>
    /// <returns></returns>
    protected abstract IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync();

    /// <summary>
    /// Enumerates over the objects (files), in no particular order, writing
    /// the results to an text writer in JSON format.
    /// </summary>
    /// <param name="textWriter">The text writer to write each of the JSON objects,
    /// one per line. The caller will need to sort the results prior
    /// to comparing them.</param>
    /// <returns></returns>
    public async Task WriteObjectsAsJsonAsync(TextWriter textWriter)
    {
        await foreach (ObjectInfo objInfo in EnumerateObjectsAsync().ConfigureAwait(false))
        {
            textWriter.WriteLine(JsonSerializer.Serialize(objInfo));
        }
    }
}
