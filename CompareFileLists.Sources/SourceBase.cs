using System.Text;

namespace CompareFileLists.Sources;

public abstract class SourceBase
{
    public abstract string Description { get; }

    /// <summary>
    /// Enumerates over the objects (files), in no particular order. The caller
    /// will need to sort the results prior to comparing them.
    /// </summary>
    /// <returns></returns>
    public abstract IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync();

    /// <summary>
    /// Enumerates over the objects (files), in no particular order, writing
    /// the results to an output stream in JSON format.
    /// </summary>
    /// <param name="outputStream">The stream to write each of the JSON objects,
    /// one per line. The caller will need to sort the results prior 
    /// to comparing them.</param>
    /// <returns></returns>
    public async Task WriteObjectsAsJsonAsync(Stream outputStream)
    {
        await foreach (ObjectInfo objInfo in EnumerateObjectsAsync().ConfigureAwait(false))
        {
            outputStream.Write(Encoding.UTF8.GetBytes($"{objInfo}{Environment.NewLine}"));
        }
    }

    /// <summary>
    /// Enumerates over the objects (files), in no particular order, writing
    /// the results to an text writer in JSON format.
    /// </summary>
    /// <param name="textWriter">The text writer to write each of the JSON objects,
    /// one per line. The caller will need to sort the results prior
    /// to comparing them.</param>
    /// <returns></returns>
    public async Task WriteObjectsAsync(TextWriter textWriter)
    {
        await foreach (ObjectInfo objInfo in EnumerateObjectsAsync().ConfigureAwait(false))
        {
            textWriter.WriteLine(objInfo);
        }
    }
}
