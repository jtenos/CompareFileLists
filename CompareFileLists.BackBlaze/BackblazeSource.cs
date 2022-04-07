using CompareFileLists.Core;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CompareFileLists.Backblaze;

public class BackblazeSource
    : SourceBase
{
    private const string AUTH_URL = "https://api.backblazeb2.com/b2api/v2/b2_authorize_account";
    private const string API_PATH = "/b2api/v2/b2_list_file_names";

    public BackblazeSource(string applicationKeyID, string applicationKey, string bucketID)
    {
        ApplicationKeyID = applicationKeyID;
        ApplicationKey = applicationKey;
        BucketID = bucketID;
    }

    private string ApplicationKeyID { get; }
    private string ApplicationKey { get; }
    private string BucketID { get; }

    protected override async IAsyncEnumerable<ObjectInfo> EnumerateObjectsAsync()
    {
        AuthorizationResponse authResponse = Authenticate();

        FileCollectionResult fileCollection;
        string? startFileName = null;

        do
        {
            #pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"{authResponse.ApiUrl}{API_PATH}");
            #pragma warning restore SYSLIB0014 // Type or member is obsolete

            object requestBody = new
            {
                bucketId = BucketID,
                maxFileCount = 10000,
                startFileName
            };
            string body = JsonSerializer.Serialize(requestBody);
            var data = Encoding.UTF8.GetBytes(body);
            webRequest.Method = "POST";
            webRequest.Headers.Add("Authorization", authResponse.AuthorizationToken);
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.ContentLength = data.Length;
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            WebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();
            fileCollection = JsonSerializer.Deserialize<FileCollectionResult>(json)!;
            foreach (var file in fileCollection.Files)
            {
                string key = Regex.Replace(file.FileName, @"\.bzEmpty$", "");
                yield return new(key, file.Size);
            }

            startFileName = fileCollection.NextFileName;
            //Console.WriteLine($"startFileName={startFileName}");
        } while (startFileName is { Length: > 0 });

        await Task.CompletedTask;
    }

    private AuthorizationResponse Authenticate()
    {
        #pragma warning disable SYSLIB0014 // Type or member is obsolete
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(AUTH_URL);
        #pragma warning restore SYSLIB0014 // Type or member is obsolete

        string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApplicationKeyID}:{ApplicationKey}"));
        webRequest.Headers.Add("Authorization", $"Basic {credentials}");
        webRequest.ContentType = "application/json; charset=utf-8";
        WebResponse response = (HttpWebResponse)webRequest.GetResponse();
        string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
        response.Close();
        return JsonSerializer.Deserialize<AuthorizationResponse>(json)!;
    }
}
