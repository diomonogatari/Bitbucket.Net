using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Bitbucket.Net.Benchmarks.Config;

/// <summary>
/// Default benchmark configuration for Bitbucket.Net benchmarks.
/// </summary>
public class DefaultBenchmarkConfig : ManualConfig
{
    public DefaultBenchmarkConfig()
    {
        // Use short run for faster iteration during development
        // Switch to Job.Default for official benchmark runs
        AddJob(Job.ShortRun
            .WithWarmupCount(3)
            .WithIterationCount(5));

        // Memory diagnostics to track allocations
        AddDiagnoser(MemoryDiagnoser.Default);

        // Columns
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.StdErr);
        AddColumn(StatisticColumn.StdDev);
        AddColumn(StatisticColumn.Median);
        AddColumn(StatisticColumn.Min);
        AddColumn(StatisticColumn.Max);
        AddColumn(StatisticColumn.OperationsPerSecond);

        // Exporters
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);
        AddExporter(CsvExporter.Default);

        // Logger
        AddLogger(ConsoleLogger.Default);

        // Summary style
        WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend));
    }
}

/// <summary>
/// Quick benchmark configuration for development/testing.
/// Uses minimal iterations for fast feedback.
/// </summary>
public class QuickBenchmarkConfig : ManualConfig
{
    public QuickBenchmarkConfig()
    {
        AddJob(Job.Dry);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddLogger(ConsoleLogger.Default);
        AddExporter(MarkdownExporter.Console);
    }
}

/// <summary>
/// Full benchmark configuration for official benchmark runs.
/// Uses default BenchmarkDotNet settings for accurate results.
/// </summary>
public class FullBenchmarkConfig : ManualConfig
{
    public FullBenchmarkConfig()
    {
        AddJob(Job.Default);
        AddDiagnoser(MemoryDiagnoser.Default);
        
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.StdErr);
        AddColumn(StatisticColumn.StdDev);
        AddColumn(StatisticColumn.Median);
        AddColumn(StatisticColumn.P95);
        AddColumn(StatisticColumn.OperationsPerSecond);
        
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);
        AddExporter(CsvExporter.Default);
        
        AddLogger(ConsoleLogger.Default);
        
        WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend));
    }
}
