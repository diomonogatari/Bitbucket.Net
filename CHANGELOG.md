# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-12

### Breaking Changes

- **`IReadOnlyList<T>` return types**: All buffered collection methods
  now return `Task<IReadOnlyList<T>>` instead of `Task<IEnumerable<T>>`.
  Consumers assigning results to `IEnumerable<T>` are unaffected;
  consumers assigning to `List<T>` must add `.ToList()` or change the
  variable type.
- **Init-only model properties**: 377 properties across 106
  response model classes converted from `{ get; set; }` to
  `{ get; init; }`. Models used as request bodies (32 files, 98
  properties) retain mutable setters to allow consumer construction.
  Consumers that assign model properties after construction must move
  to object-initializer syntax.
- **Dedicated request DTOs**: Write operations now accept
  purpose-built request DTOs instead of reusing response models or
  inline parameters. 13 request DTOs created:
  `CreateProjectRequest`, `UpdateProjectRequest`,
  `CreateRepositoryRequest`, `ForkRepositoryRequest`,
  `CreatePullRequestRequest`, `UpdatePullRequestRequest`,
  `CreateBranchRequest`, `CreateTaskRequest`, `UpdateTaskRequest`,
  `CreateWebHookRequest`, `UpdateWebHookRequest`,
  `AssociateBuildStatusRequest`, `MergePullRequestRequest`.
  Methods affected: `CreateProjectAsync`, `UpdateProjectAsync`,
  `CreateProjectRepositoryAsync`, `CreateProjectRepositoryForkAsync`,
  `CreatePullRequestAsync`, `UpdatePullRequestAsync`,
  `MergePullRequestAsync`, `CreateBranchAsync`, `CreateTaskAsync`,
  `UpdateTaskAsync`, `CreateProjectRepositoryWebHookAsync`,
  `UpdateProjectRepositoryWebHookAsync`,
  `AssociateBuildStatusWithCommitAsync`.
  Request DTOs use `required` and `init`-only properties,
  exposing only API-relevant fields.

### Changed

- **Source-gen-only deserialization**: Removed the
  `JsonUnknownTypeHandling.JsonNode` reflection fallback from the
  read-path `JsonSerializerOptions`. Deserialization now uses only
  the source-generated `BitbucketJsonContext`, eliminating
  reflection-based metadata and improving trim safety. A separate
  `s_writeJsonOptions` retains a reflection fallback solely for
  serializing anonymous types in request bodies.
- **Stream-based deserialization**: `ReadResponseContentAsync<T>`
  now calls `JsonSerializer.DeserializeAsync` directly on the HTTP
  response stream instead of reading the body into a `string` first.
  Eliminates a full UTF-16 string copy per response.
- **FrozenDictionary enum lookups**: All 25 enum-to-string
  mapping dictionaries in `BitbucketHelpers` converted from
  `Dictionary<TEnum, string>` to `FrozenDictionary<TEnum, string>`.
  Added reverse `FrozenDictionary<string, TEnum>` with
  `StringComparer.OrdinalIgnoreCase` for O(1) string-to-enum lookups.
- **Consolidated enum mappings**: Introduced generic
  `EnumMap<TEnum>` as the single source of truth for all 25
  enum-to-string mappings. Replaced 13 individual converter
  subclasses with a unified `BitbucketEnumConverterFactory`.
  Slimmed `BitbucketHelpers.cs` from ~1,100 to ~490 lines.
  Added public `ToApiString()` extension methods on all enum types.
- **Frozen `JsonSerializerOptions`**: Both `s_jsonOptions`
  and `s_writeJsonOptions` are now explicitly frozen via
  `MakeReadOnly()` at construction time, preventing accidental
  mutation from any thread.
- **`IReadOnlyList<T>` return types**: All 27 buffered
  collection methods now return `Task<IReadOnlyList<T>>` instead of
  `Task<IEnumerable<T>>`, communicating immutability and preventing
  multiple-enumeration bugs.
- **Paginated endpoint helpers**: Introduced shared `GetPagedAsync<T>`
  and `GetPagedStreamAsync<T>` methods, replacing ~300 lines of
  duplicated pagination logic across 82 endpoints.
- **Dead code removal**: Removed unused `UnixDateTimeExtensions`,
  `DictionaryExtensions`, uncalled `BitbucketHelpers` conversion
  methods, and redundant `ExecuteAsync` overloads.

### Added

- **NuGet package metadata improvements**: Version bumped to
  1.0.0. Added `PackageIcon` (128×128 placeholder in `assets/icon.png`),
  expanded `PackageTags` (`rest-api`, `api-client`, `atlassian`, `sdk`,
  `dotnet`), and conditional icon inclusion with `Condition="Exists(…)"`.
  `dotnet pack` now produces `BitbucketServer.Net.1.0.0.nupkg` with
  README, icon, and full metadata.
- **Rate-limit headers on `BitbucketRateLimitException`**:
  HTTP 429 exceptions now expose `RetryAfter`, `RateLimit`,
  `RateLimitRemaining`, and `RateLimitReset` properties parsed from
  standard rate-limit response headers. Gracefully returns `null` for
  missing or unparseable headers.
- **`PagedResultsReader` zero-allocation metadata parser**:
  Internal `Utf8JsonReader`-based parser that extracts pagination
  metadata (`isLastPage`, `nextPageStart`, `start`, `limit`, `size`)
  directly from UTF-8 bytes without deserializing the full payload.
  Opt-in for hot-path streaming scenarios.
- **`IDisposable` on `BitbucketClient`**: The client now
  implements `IDisposable` with ownership tracking. Clients created
  via the `(string url, ...)` constructors own and dispose the
  underlying `FlurlClient`. Clients created via `(IFlurlClient, ...)`
  or `(HttpClient, ...)` do not dispose the injected client. All public
  methods throw `ObjectDisposedException` after disposal.
- **`ExecuteAsync` centralised error handling**: New
  `ExecuteAsync<TResult>`, `ExecuteAsync` (bool), and
  `ExecuteWithNoContentAsync` methods that wrap HTTP call + response
  handling in a single call, reducing boilerplate in API methods.
- **Input validation guards**: ~130 public methods now
  validate URL-path string parameters (`projectKey`, `repositorySlug`,
  `commitId`, `hookKey`, `userSlug`, etc.) with
  `ArgumentException.ThrowIfNullOrWhiteSpace()` at method entry.
  Prevents malformed URLs and confusing server-side errors.
- **Fluent query builders**: New `PullRequestQueryBuilder`,
  `CommitQueryBuilder`, `BranchQueryBuilder`, and `ProjectQueryBuilder`
  classes providing a fluent API for complex queries. Entry points:
  `client.PullRequests(...)`, `client.Commits(...)`,
  `client.Branches(...)`, `client.Projects()`. Each builder supports
  `GetAsync()` for buffered results and `StreamAsync()` for
  `IAsyncEnumerable<T>` streaming. Existing flat methods are unchanged.
- **OpenTelemetry tracing**: HTTP calls are traced via an internal
  `ActivitySource` named `"Bitbucket.Net"`. Add it to your
  `TracerProviderBuilder` to get per-request spans with method, URL,
  and status code attributes.
- **`IBitbucketClient` interface**: Extracted from `BitbucketClient`
  for dependency injection and unit testing. The interface is composed
  of 12 domain-specific sub-interfaces (`IProjectOperations`,
  `IRepositoryOperations`, `IPullRequestOperations`, etc.) so
  consumers can depend on only the slice they need.
- **Code search**: `SearchCodeAsync` and `SearchCodeStreamAsync`
  methods wrapping the Bitbucket Server code search REST API.

### Testing

- Added `SourceGenCoverageTests` validating all model types are
  registered in `BitbucketJsonContext`.
- Added `BitbucketClientDisposeTests` (6 tests) covering disposal
  semantics and ownership tracking.
- Added `FluentQueryBuilderMockTests` (12 tests) covering all four
  query builders with default parameters, custom options, streaming,
  and input validation.
- Added `ArchitecturalTests` verifying all HTTP calls have error
  handlers (`HandleResponseAsync`, `ExecuteAsync`, or `StatusCode`),
  that `JsonSerializerOptions` are explicitly frozen, and that every
  `await` uses `ConfigureAwait(false)`.
- Added `InputValidationTests` (17 parameterized theories) covering
  null/empty/whitespace rejection for key path-segment parameters.
- Total test count: 749.

## [0.3.0] - 2026-02-010

### Added

- **Code search**: `SearchCodeAsync` and `SearchCodeStreamAsync`
  methods wrapping the Bitbucket Server code search REST API
  (`/rest/search/latest/search`).

## [0.2.0] - 2026-02-08

### Breaking Changes

- **Exception handling**: The library now throws `BitbucketApiException` (and its typed subtypes) instead of `FlurlHttpException`. Consumers catching `FlurlHttpException` must update their catch blocks.
- **`Comment` model**: No longer inherits from `PullRequestInfo`. Properties such as `Title`, `Description`, `FromRef`, `ToRef`, `Locked`, and `Reviewers` are removed from `Comment`. These were always null/default on comments and should not have been exposed.
- **`Comment.State`**: Changed from `new string?` (hiding a `PullRequestStates` enum) to a plain `string?` property.
- **Global Flurl configuration removed**: The library no longer calls `FlurlHttp.Clients.WithDefaults()`. Other Flurl consumers in the same process are no longer affected.

### Added

- **SourceLink**: Consumers can step into library source during debugging.
- **Symbol packages**: `.snupkg` published alongside `.nupkg`.
- **XML documentation**: IntelliSense documentation included in the NuGet package. Model classes now have comprehensive `<summary>` and `<param>` XML docs.
- **New streaming methods**:
  - `GetPullRequestActivitiesStreamAsync`
  - `GetPullRequestChangesStreamAsync`
  - `GetPullRequestCommentsStreamAsync`
  - `GetPullRequestParticipantsStreamAsync`
  - `GetPullRequestTasksStreamAsync`
  - `GetPullRequestBlockerCommentsStreamAsync`
  - `GetDashboardPullRequestsStreamAsync`
  - `GetInboxPullRequestsStreamAsync`
  - `GetProjectRepositoryTagsStreamAsync`
  - `GetChangesStreamAsync`
  - `GetCommitChangesStreamAsync`
- **`global.json`**: SDK version pinned for reproducible builds.
- **`Directory.Build.props`**: Centralized build configuration (TFM, language version, nullable, warnings-as-errors).
- **Code coverage**: CI collects and reports test coverage.
- **File splitting**: Monolithic `Core/Projects/BitbucketClient.cs` (4 491 lines) split into 10 focused partial-class files by domain (projects, repositories, branches, commits, compare, pull requests, PR comments, PR details, tasks, repository settings).

### Fixed

- **Typed exceptions now fire correctly** for all HTTP error responses. Previously, Flurl intercepted errors before the custom handling could run.
- **`CancellationToken` propagation**: Helper methods now pass the token to underlying HTTP calls.
- **`PullRequest.ToString()`**: No longer throws `NullReferenceException` when `Author` or `Author.User` is null.
- **`Participant.ToString()`**: Same null-safety fix.

### Changed

- Removed commented-out `Avatar` property from `ProjectDefinition`.
- Fixed duplicate `<summary>` XML doc tag on `GetRepositoriesStreamAsync`.

### Testing

- Added streaming endpoint mock tests covering all 20 streaming methods (single-page, multi-page, empty result scenarios)
- Added diff streaming tests for commit, repository, compare, and PR diffs (single, multiple, empty)
- Added MCP extension method tests for `StreamDiffsWithLimitsAsync` and `TakeDiffsWithLimitsAsync`
- Added cancellation token propagation tests (pre-cancelled tokens for buffered, streaming, and diff methods; mid-stream cancellation)
- Added DI constructor integration tests for `HttpClient` and `IFlurlClient` injection paths (CRUD, error handling, streaming, auth header verification)
- Introduced paginated fixture data (`projects-page1.json`, `projects-page2.json`, etc.) and `SetupPagedEndpoint` helper for multi-page mock tests
- Total test count increased from 633 to 696 (+63 new tests)

## [0.1.0-beta.1] - 2026-02-06 (pre-release)

### Notes

First public pre-release of the modernized fork. Superseded by 0.2.0.

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
