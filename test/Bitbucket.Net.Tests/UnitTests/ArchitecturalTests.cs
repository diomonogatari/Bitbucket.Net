using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

/// <summary>
/// Architectural tests that verify structural invariants across the codebase.
/// These tests catch unsafe patterns that cannot be detected at compile time.
/// </summary>
public class ArchitecturalTests
{
    private static readonly string s_sourceDir = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "Bitbucket.Net"));

    /// <summary>
    /// Verifies that the static JsonSerializerOptions instances are explicitly
    /// frozen (read-only), preventing accidental mutation from any thread.
    /// </summary>
    [Fact]
    public void JsonSerializerOptions_AreExplicitlyFrozen()
    {
        var clientType = typeof(BitbucketClient);
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        var readOptions = clientType.GetField("s_jsonOptions", bindingFlags)?.GetValue(null) as JsonSerializerOptions;
        Assert.NotNull(readOptions);
        Assert.True(readOptions.IsReadOnly, "s_jsonOptions should be explicitly frozen via MakeReadOnly()");

        var writeOptions = clientType.GetField("s_writeJsonOptions", bindingFlags)?.GetValue(null) as JsonSerializerOptions;
        Assert.NotNull(writeOptions);
        Assert.True(writeOptions.IsReadOnly, "s_writeJsonOptions should be explicitly frozen via MakeReadOnly()");
    }

    /// <summary>
    /// Verifies that every HTTP call in BitbucketClient partial class files
    /// has a corresponding error handler (HandleResponseAsync, HandleErrorsAsync,
    /// or ExecuteAsync). This prevents silent swallowing of HTTP errors since
    /// BitbucketClient uses AllowAnyHttpStatus().
    /// </summary>
    [Fact]
    public void AllHttpCalls_HaveCorrespondingErrorHandlers()
    {
        var sourceFiles = Directory.GetFiles(s_sourceDir, "BitbucketClient*.cs", SearchOption.AllDirectories);
        Assert.NotEmpty(sourceFiles);

        var httpCallPattern = new Regex(@"\.(Get|Post|Put|Delete|Patch|Send)Async\(", RegexOptions.Compiled);
        var errorHandlerPattern = new Regex(@"(HandleResponseAsync|HandleErrorsAsync|ExecuteAsync|ExecuteWithNoContentAsync|response\.StatusCode)", RegexOptions.Compiled);

        var failures = new List<string>();

        foreach (var file in sourceFiles)
        {
            string content = File.ReadAllText(file);
            int httpCalls = httpCallPattern.Matches(content).Count;
            int errorHandlers = errorHandlerPattern.Matches(content).Count;

            if (httpCalls > errorHandlers)
            {
                string fileName = Path.GetRelativePath(s_sourceDir, file);
                failures.Add($"{fileName}: {httpCalls} HTTP calls but only {errorHandlers} error handlers");
            }
        }

        Assert.True(
            failures.Count == 0,
            $"Unhandled HTTP calls detected (every HTTP call must use HandleResponseAsync, HandleErrorsAsync, or ExecuteAsync):\n{string.Join('\n', failures)}");
    }
}