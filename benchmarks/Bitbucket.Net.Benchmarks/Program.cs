using BenchmarkDotNet.Running;
using Bitbucket.Net.Benchmarks.Config;
using Bitbucket.Net.Benchmarks.Response;
using Bitbucket.Net.Benchmarks.Serialization;
using Bitbucket.Net.Benchmarks.Streaming;
using Bitbucket.Net.Benchmarks.ZeroCopy;

namespace Bitbucket.Net.Benchmarks;

/// <summary>
/// Entry point for Bitbucket.Net performance benchmarks.
/// 
/// Usage:
///   dotnet run -c Release                           # Interactive benchmark picker
///   dotnet run -c Release -- --filter *Json*        # Run JSON benchmarks only
///   dotnet run -c Release -- --filter *Streaming*   # Run streaming benchmarks only
///   dotnet run -c Release -- --filter *Response*    # Run response handling benchmarks only
///   dotnet run -c Release -- --filter *ZeroCopy*    # Run zero-copy benchmarks only
///   dotnet run -c Release -- --list flat            # List all available benchmarks
///   dotnet run -c Release -- --job dry              # Quick dry run
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            // Interactive mode - let user pick benchmarks
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
        else if (args.Contains("--all"))
        {
            // Run all benchmarks
            RunAllBenchmarks();
        }
        else
        {
            // Pass through to BenchmarkDotNet's argument parser
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    private static void RunAllBenchmarks()
    {
        Console.WriteLine("Running all Bitbucket.Net benchmarks...");
        Console.WriteLine();

        var config = new DefaultBenchmarkConfig();

        Console.WriteLine("=== JSON Serialization Benchmarks ===");
        BenchmarkRunner.Run<JsonSerializationBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("=== Streaming vs Buffered Benchmarks ===");
        BenchmarkRunner.Run<StreamingBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("=== Response Handling Benchmarks ===");
        BenchmarkRunner.Run<ResponseHandlingBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("=== Zero-Copy Benchmarks ===");
        BenchmarkRunner.Run<ZeroCopyBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("=== PagedResultsReader Benchmarks ===");
        BenchmarkRunner.Run<PagedResultsReaderBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("All benchmarks completed!");
        Console.WriteLine("Results are available in the BenchmarkDotNet.Artifacts folder.");
    }
}