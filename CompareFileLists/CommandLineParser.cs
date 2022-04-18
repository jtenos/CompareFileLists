using CompareFileLists.AzureBlob;
using CompareFileLists.Backblaze;
using CompareFileLists.Core;
using CompareFileLists.FileSystem;
using CompareFileLists.S3;
using System.CommandLine;

namespace CompareFileLists;

internal static class CommandLineParser
{
    public static (SourceBase FirstSource, SourceBase SecondSource) Parse(string[] args)
    {
        Option<FileProvider> firstProviderOption = new("--first-provider", "First provider type");
        Option<FileProvider> secondProviderOption = new("--second-provider", "Second provider type");

        RootCommand rootCommand = new()
        {
            firstProviderOption,
            secondProviderOption
        };

        rootCommand.Description = "Compare File Lists";

        SourceBase firstSource = default!;
        SourceBase secondSource = default!;

        rootCommand.SetHandler<FileProvider, FileProvider>(Execute, firstProviderOption, secondProviderOption);
        rootCommand.Invoke(args);

        return (firstSource, secondSource);

        void Execute(FileProvider firstProvider, FileProvider secondProvider)
        {
            // TODO Pull the rest of the data from the console
            firstSource = firstProvider switch
            {
                FileProvider.AmazonS3 => new S3Source(),
                FileProvider.FileSystem => new FileSystemSource(""),
                FileProvider.AzureBlob => new AzureBlobSource("", ""),
                FileProvider.BackblazeB2 => new BackblazeSource("", "", ""),
                _ => throw new ApplicationException("Invalid first source")
            };

            secondSource = secondProvider switch
            {
                FileProvider.AmazonS3 => new S3Source(),
                FileProvider.FileSystem => new FileSystemSource(""),
                FileProvider.AzureBlob => new AzureBlobSource("", ""),
                FileProvider.BackblazeB2 => new BackblazeSource("", "", ""),
                _ => throw new ApplicationException("Invalid first source")
            };
        }
    }
}
