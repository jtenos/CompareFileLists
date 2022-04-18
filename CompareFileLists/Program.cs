using CompareFileLists;
using CompareFileLists.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<FileListComparison>();
        services.AddSingleton<Program>();
    })
    .Build();

await host.Services.GetRequiredService<Program>().GoAsync(args);

partial class Program
{
    public async Task GoAsync(string[] args)
    {
        (SourceBase firstSource, SourceBase secondSource) = CommandLineParser.Parse(args);
        Console.WriteLine(firstSource.GetType().Name);
        Console.WriteLine(secondSource.GetType().Name);
        await Task.CompletedTask;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("TODO: Show the help screen here");
    }
}
