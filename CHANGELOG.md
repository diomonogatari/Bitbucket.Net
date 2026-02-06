# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0-beta.1] - 2026-02-06

### Notes

This is the first public release of the modernized fork by
[diomonogatari](https://github.com/diomonogatari).
The version number intentionally starts at `0.x` to signal that the
library is **not yet production-ready** &mdash; it is being dog-fooded
in an MCP Server for on-prem Bitbucket Server but not every endpoint
has been exhaustively tested.

The original [lvermeulen/Bitbucket.Net](https://github.com/lvermeulen/Bitbucket.Net)
shipped up to 0.5.0 on NuGet; this fork is versioned independently.

## [2.0.0] - 2025-11-28 (internal)

### ⚠️ Breaking Changes

- **Target Framework**: Upgraded from .NET Framework 4.5.2 / .NET Standard 1.4 to **.NET 10.0**
- **JSON Serializer**: Migrated from Newtonsoft.Json to **System.Text.Json** for improved performance
- **Flurl.Http**: Upgraded from 2.4.2 to **4.0.2** (major API changes)
- **Exception Handling**: `InvalidOperationException` replaced with typed `BitbucketApiException` hierarchy (see [Typed Exceptions](#typed-exception-hierarchy) below)
- **Branch.Metadata**: Property type changed from `dynamic` to `JsonElement?` due to System.Text.Json migration. Use the strongly-typed `BranchMetadata` property instead for common metadata access (ahead/behind, build status, PR info).
- Removed dependency on Newtonsoft.Json 12.0.2 (had [CVE-2024-21907](https://github.com/advisories/GHSA-5crp-9r3c-p9vr))

### Added

#### CancellationToken Support
- All async methods now accept an optional `CancellationToken` parameter
- Enables graceful cancellation of long-running operations
- Fully propagated to underlying HTTP calls

#### IAsyncEnumerable Streaming
- New streaming variants for paginated endpoints that yield items as they arrive:
  - `GetProjectsStreamAsync()`
  - `GetProjectRepositoriesStreamAsync()`
  - `GetRepositoriesStreamAsync()`
  - `GetBranchesStreamAsync()`
  - `GetPullRequestsStreamAsync()`
  - `GetPullRequestCommitsStreamAsync()`
  - `GetCommitsStreamAsync()`
- Benefits:
  - Lower memory usage for large result sets
  - Faster time-to-first-result
  - Native `await foreach` support

#### Diff and File Content Streaming
- New streaming methods for large diff responses:
  - `GetCommitDiffStreamAsync()` - Stream diffs for a specific commit
  - `GetRepositoryDiffStreamAsync()` - Stream repository diffs between refs
  - `GetRepositoryCompareDiffStreamAsync()` - Stream compare diffs between branches/commits
  - `GetPullRequestDiffStreamAsync()` - Stream pull request diffs (existing, refactored)
- New raw file content streaming:
  - `GetRawFileContentStreamAsync()` - Get file content as a raw `Stream`
  - `GetRawFileContentLinesStreamAsync()` - Stream file content line by line
- Benefits:
  - Efficient handling of large diffs without buffering entire response
  - Process diff entries as they arrive
  - Reduced memory pressure for large file downloads

#### Dependency Injection Support
- New constructor: `BitbucketClient(HttpClient httpClient, string baseUrl, Func<string> getToken = null)`
  - Enables use with `IHttpClientFactory`
  - Supports Polly resilience policies (retry, circuit breaker, etc.)
  - Allows custom `DelegatingHandler` middleware
- New constructor: `BitbucketClient(IFlurlClient flurlClient, Func<string> getToken = null)`
  - For advanced Flurl configuration scenarios
  - Supports `IFlurlClientCache` for named client management

#### Typed Exception Hierarchy
- New `BitbucketApiException` base class with rich error information:
  - `StatusCode`: HTTP status code as `HttpStatusCode` enum
  - `Context`: The field or resource that caused the error
  - `Errors`: Collection of all errors from the response
  - `RequestUrl`: The URL that failed
- Specific exception types for common HTTP errors:
  - `BitbucketBadRequestException` (HTTP 400)
  - `BitbucketAuthenticationException` (HTTP 401)
  - `BitbucketForbiddenException` (HTTP 403)
  - `BitbucketNotFoundException` (HTTP 404)
  - `BitbucketConflictException` (HTTP 409)
  - `BitbucketValidationException` (HTTP 422)
  - `BitbucketRateLimitException` (HTTP 429)
  - `BitbucketServerException` (HTTP 5xx)

#### Code Quality Enforcement
- Added `Meziantou.Analyzer` to enforce library best practices
- ConfigureAwait(false) requirement enforced via MA0004 (warning level)
- Nullable reference types enabled project-wide
- EditorConfig configured with library-appropriate analyzer rules

#### Performance Benchmarks
- New benchmark project (`benchmarks/Bitbucket.Net.Benchmarks`) using BenchmarkDotNet
- Benchmark categories:
  - **JSON Serialization**: Measure System.Text.Json performance for serialization/deserialization
  - **Streaming**: Compare IAsyncEnumerable streaming vs buffered List approaches
  - **Response Handling**: Test large response processing efficiency
- Benefits:
  - Establish performance baselines for future optimizations
  - Verify performance improvements from migration to System.Text.Json
  - Catch performance regressions early

### Changed

- **Performance**: System.Text.Json provides ~2-3x faster serialization/deserialization
- **Memory**: Reduced allocations with source-generated JSON serialization options
- All model classes now use `[JsonPropertyName]` attributes instead of `[JsonProperty]`
- Internal JSON converters rewritten for System.Text.Json compatibility
- All async methods audited for `ConfigureAwait(false)` compliance

### Fixed

- Build artifacts (bin/obj) no longer tracked in git repository

### Migration Guide

#### Updating from 1.x to 2.0.0

1. **Update Target Framework**
   ```xml
   <!-- Old -->
   <TargetFramework>netstandard1.4</TargetFramework>
   
   <!-- New -->
   <TargetFramework>net10.0</TargetFramework>
   ```

2. **No Code Changes Required** for basic usage - the API remains backward compatible

3. **Optional: Use CancellationToken**
   ```csharp
   // Before
   var projects = await client.GetProjectsAsync();
   
   // After (optional improvement)
   var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
   var projects = await client.GetProjectsAsync(cancellationToken: cts.Token);
   ```

4. **Optional: Use Streaming for Large Results**
   ```csharp
   // Before - buffers all results in memory
   var allPRs = await client.GetPullRequestsAsync("PROJ", "repo");
   
   // After - streams results as they arrive
   await foreach (var pr in client.GetPullRequestsStreamAsync("PROJ", "repo"))
   {
       await ProcessAsync(pr);
   }
   ```

5. **Optional: Use Dependency Injection**
   ```csharp
   // Configure with IHttpClientFactory + Polly
   services.AddHttpClient<BitbucketClient>()
       .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)));
   
   services.AddSingleton<BitbucketClient>(sp =>
   {
       var httpClient = sp.GetRequiredService<IHttpClientFactory>()
           .CreateClient(nameof(BitbucketClient));
       return new BitbucketClient(httpClient, "https://bitbucket.example.com", () => GetToken());
   });
   ```

6. **Update Exception Handling** (Breaking Change)
   ```csharp
   // Before - catching generic InvalidOperationException
   try
   {
       var repo = await client.GetRepositoryAsync("PROJ", "repo");
   }
   catch (InvalidOperationException ex)
   {
       // Had to parse the message to determine error type
       Console.WriteLine($"Error: {ex.Message}");
   }
   
   // After - catch specific exceptions
   try
   {
       var repo = await client.GetRepositoryAsync("PROJ", "repo");
   }
   catch (BitbucketNotFoundException ex)
   {
       // Handle 404 - repository doesn't exist
       Console.WriteLine($"Repository not found: {ex.Context}");
   }
   catch (BitbucketAuthenticationException ex)
   {
       // Handle 401 - invalid credentials
       Console.WriteLine("Authentication failed. Check your credentials.");
   }
   catch (BitbucketForbiddenException ex)
   {
       // Handle 403 - insufficient permissions
       Console.WriteLine($"Access denied: {string.Join(", ", ex.Errors.Select(e => e.Message))}");
   }
   catch (BitbucketApiException ex)
   {
       // Catch-all for other API errors
       Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
       Console.WriteLine($"Request URL: {ex.RequestUrl}");
   }
   ```

---

## [1.x] - Previous Releases

See [GitHub Releases](https://github.com/lvermeulen/Bitbucket.Net/releases) for historical changelog.
