# Bitbucket.Net Benchmarks

Performance benchmarks for Bitbucket.Net using [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Prerequisites

- .NET 10.0 SDK or later
- Build the solution in Release mode for accurate results

## Running Benchmarks

### Run All Benchmarks

```bash
cd benchmarks/Bitbucket.Net.Benchmarks
dotnet run -c Release
```

Select option `1` from the menu to run all benchmarks.

### Run Specific Benchmark Category

```bash
dotnet run -c Release
```

Then select from the menu:

- `2` - JSON Serialization benchmarks
- `3` - Streaming benchmarks
- `4` - Response Handling benchmarks

### Run with Command Line Arguments

You can also run specific benchmarks directly:

```bash
# Run JSON serialization benchmarks only
dotnet run -c Release -- --filter "*JsonSerialization*"

# Run streaming benchmarks only
dotnet run -c Release -- --filter "*Streaming*"

# Run response handling benchmarks only
dotnet run -c Release -- --filter "*ResponseHandling*"
```

## Benchmark Categories

### JSON Serialization (`Serialization/`)

Measures System.Text.Json performance for common operations:

- `DeserializeRepository` - Deserialize a single repository with nested objects
- `DeserializePagedResults` - Deserialize paged API responses
- `SerializeRepository` - Serialize a repository object
- `SerializeProject` - Serialize a project object

### Streaming (`Streaming/`)

Compares IAsyncEnumerable streaming vs traditional buffered approaches:

- `StreamingEnumeration` - Process items using `yield return` (streaming)
- `BufferedEnumeration` - Process items by collecting to a List first (buffered)

Tests memory efficiency improvements from Issue #2 (IAsyncEnumerable Streaming).

### Response Handling (`Response/`)

Tests handling of various response sizes:

- `SmallResponse` - ~1KB response
- `MediumResponse` - ~100KB response
- `LargeResponse` - ~1MB response

Measures allocation patterns and throughput for different payload sizes.

## Understanding Results

BenchmarkDotNet produces detailed reports including:

| Column | Description |
|--------|-------------|
| Mean | Average execution time |
| Error | 99.9% confidence interval |
| StdDev | Standard deviation |
| Ratio | Relative performance compared to baseline |
| Allocated | Memory allocated per operation |

Results are exported to:

- `BenchmarkDotNet.Artifacts/results/` (CSV, HTML, Markdown)

## Best Practices

1. **Always use Release mode** - Debug builds include debugging overhead
2. **Close other applications** - Reduce system noise for consistent results
3. **Run multiple iterations** - BenchmarkDotNet handles this automatically
4. **Compare against baseline** - Use `[Benchmark(Baseline = true)]` for comparisons

## Adding New Benchmarks

1. Create a new class in the appropriate category folder
2. Add `[MemoryDiagnoser]` attribute for memory measurements
3. Use `[Config(typeof(BenchmarkConfig))]` for consistent configuration
4. Add `[Benchmark]` attribute to benchmark methods
5. Update `Program.cs` to include the new benchmark class

Example:

```csharp
[MemoryDiagnoser]
[Config(typeof(BenchmarkConfig))]
public class MyNewBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        // Initialize test data
    }

    [Benchmark(Baseline = true)]
    public void BaselineMethod()
    {
        // Baseline implementation
    }

    [Benchmark]
    public void ImprovedMethod()
    {
        // Implementation to compare
    }
}
```

## CI Integration

For CI/CD pipelines, run benchmarks without the interactive menu:

```bash
dotnet run -c Release -- --filter "*" --exporters csv html markdown
```

Results can be archived as build artifacts for trend analysis.
