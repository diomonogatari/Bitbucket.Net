# Bitbucket.Net

[![NuGet](https://img.shields.io/nuget/v/BitbucketServer.Net.svg)](https://www.nuget.org/packages/BitbucketServer.Net)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BitbucketServer.Net.svg)](https://www.nuget.org/packages/BitbucketServer.Net)
[![CI](https://github.com/diomonogatari/Bitbucket.Net/actions/workflows/ci.yml/badge.svg)](https://github.com/diomonogatari/Bitbucket.Net/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/diomonogatari/Bitbucket.Net/branch/main/graph/badge.svg)](https://codecov.io/gh/diomonogatari/Bitbucket.Net)
[![license](https://img.shields.io/github/license/diomonogatari/Bitbucket.Net.svg?maxAge=2592000)](https://github.com/diomonogatari/Bitbucket.Net/blob/main/LICENSE)
![](https://img.shields.io/badge/.net-10.0-yellowgreen.svg)
![](https://img.shields.io/badge/status-1.0.0_(stable)-brightgreen.svg)

Modernized C# client for **Bitbucket Server** (Stash) REST API.

## Contributing

Development setup (including the pre-commit formatting hook) is documented in
[CONTRIBUTING.md](CONTRIBUTING.md).

> **Fork notice** — This is an actively maintained fork of
> [lvermeulen/Bitbucket.Net](https://github.com/lvermeulen/Bitbucket.Net),
> which appears to be abandoned (last release 2020).
> The 1.0.0 API surface is stable; breaking changes follow semver.
> The library is used in production by the author (as the backend for
> an MCP Server talking to on-prem Bitbucket Server), but not every
> endpoint has been verified against a live instance.
> Contributions, bug reports, and feedback are welcome.

### What changed from the original

- .NET 10 target (dropped .NET Framework / .NET Standard)
- `System.Text.Json` with source generation (no runtime reflection)
- `CancellationToken` on every async method
- `IAsyncEnumerable` streaming for paginated endpoints
- Streaming diffs and raw file content
- Typed exception hierarchy (`BitbucketNotFoundException`, etc.)
- `IHttpClientFactory` / DI-friendly constructors
- `IDisposable` with ownership tracking
- `IBitbucketClient` decomposed into 12 domain-specific sub-interfaces
- Fluent query builders for pull requests, commits, branches, and projects
- Dedicated request DTOs for write operations
- Input validation on all public API methods
- OpenTelemetry tracing via `ActivitySource`
- Bitbucket Server 9.0+ blocker-comment (task) support with legacy fallback
- Flurl.Http 4.x

If you're looking for Bitbucket Cloud API, try [this repository](https://github.com/lvermeulen/Bitbucket.Cloud.Net).

## Installation

```bash
dotnet add package BitbucketServer.Net
```

## Usage

### Basic Authentication

```csharp
var client = new BitbucketClient("https://bitbucket.example.com", "username", "password");
```

### Token Authentication

```csharp
var client = new BitbucketClient("https://bitbucket.example.com", () => GetAccessToken());
```

### Resource management

`BitbucketClient` implements `IDisposable`. Clients created with a URL
own the underlying HTTP connection and dispose it:

```csharp
using var client = new BitbucketClient("https://bitbucket.example.com", "user", "pass");
var projects = await client.GetProjectsAsync();
```

When you inject an `HttpClient` or `IFlurlClient`, the caller retains
ownership; the client will not dispose it.

### Dependency Injection with IHttpClientFactory

For production scenarios, you can inject an externally managed `HttpClient` to leverage `IHttpClientFactory` for connection pooling, resilience policies, and centralized configuration.

#### Standard resilience (recommended)

The simplest approach uses `Microsoft.Extensions.Http.Resilience` which provides
retry, circuit breaker, and timeout out of the box:

```csharp
// Requires: dotnet add package Microsoft.Extensions.Http.Resilience

services.AddHttpClient<BitbucketClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(2);
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
});

// Register IBitbucketClient for dependency injection
services.AddSingleton<IBitbucketClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient(nameof(BitbucketClient));

    return new BitbucketClient(
        httpClient,
        "https://bitbucket.example.com",
        () => sp.GetRequiredService<ITokenProvider>().GetToken());
});
```

#### Custom resilience pipeline

For fine-grained control over which responses trigger retries:

```csharp
services.AddHttpClient<BitbucketClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(2);
})
.AddResilienceHandler("bitbucket", builder =>
{
    builder
        .AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(1),
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests
                                || r.StatusCode >= HttpStatusCode.InternalServerError)
        })
        .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            BreakDuration = TimeSpan.FromSeconds(15),
        })
        .AddTimeout(TimeSpan.FromSeconds(30));
});
```

### Advanced: Using IFlurlClient

For fine-grained control over Flurl's configuration:

```csharp
services.AddSingleton<IFlurlClientCache>(sp => new FlurlClientCache()
    .Add("Bitbucket", "https://bitbucket.example.com", builder => builder
        .WithSettings(s => s.Timeout = TimeSpan.FromMinutes(5))
        .WithHeader("X-Custom-Header", "value")));

services.AddSingleton<IBitbucketClient>(sp =>
{
    var flurlClient = sp.GetRequiredService<IFlurlClientCache>().Get("Bitbucket");
    return new BitbucketClient(flurlClient, () => GetToken());
});
```

### Streaming with IAsyncEnumerable

For memory-efficient processing of large result sets, use the streaming variants:

```csharp
// Stream projects without buffering all pages in memory
await foreach (var project in client.GetProjectsStreamAsync())
{
    Console.WriteLine(project.Name);
}

// With cancellation support
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
await foreach (var pr in client.GetPullRequestsStreamAsync("PROJ", "repo", cancellationToken: cts.Token))
{
    await ProcessPullRequestAsync(pr);
}

// Stream PR activities
await foreach (var activity in client.GetPullRequestActivitiesStreamAsync(
    "PROJ", "repo", pullRequestId: 42))
{
    ProcessActivity(activity);
}

// Stream dashboard PRs
await foreach (var pr in client.GetDashboardPullRequestsStreamAsync())
{
    Console.WriteLine($"#{pr.Id}: {pr.Title}");
}
```

### Fluent query builders

For endpoints with many optional filters, query builders provide a typed
alternative to the flat method signatures:

```csharp
var openPRs = await client.PullRequests("PROJ", "repo")
    .InState(PullRequestStates.Open)
    .OrderBy(PullRequestOrders.Newest)
    .PageSize(25)
    .GetAsync();

// Streaming variant
await foreach (var pr in client.PullRequests("PROJ", "repo")
    .InState(PullRequestStates.Open)
    .StreamAsync())
{
    Console.WriteLine(pr.Title);
}
```

Builders are available for pull requests, commits, branches, and projects.
The original flat methods still work and are not deprecated.

### Exception handling

Typed exceptions give you precise control over error handling:

```csharp
try
{
    var repo = await client.GetRepositoryAsync("PROJ", "repo");
}
catch (BitbucketNotFoundException ex)
{
    Console.WriteLine($"Repository not found: {ex.Context}");
}
catch (BitbucketAuthenticationException)
{
    Console.WriteLine("Invalid credentials");
}
catch (BitbucketForbiddenException ex)
{
    Console.WriteLine($"Access denied: {ex.Message}");
}
catch (BitbucketApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
```

## Benchmarks

Performance benchmarks are available in the `benchmarks/` folder using BenchmarkDotNet:

```bash
cd benchmarks/Bitbucket.Net.Benchmarks
dotnet run -c Release
```

See [benchmarks/README.md](benchmarks/README.md) for detailed instructions.

## Features

* [X] Audit
 	- [X] Project Events
 	- [X] Repository Events
- [X] Branches
 	- [X] Create Branch
 	- [X] Delete Branch
 	- [X] Branch Info
 	- [X] Branch Model
- [X] Builds
 	- [X] Commits Build Stats
 	- [X] Commit Build Stats
 	- [X] Commit Build Status
 	- [X] Associate Build Status
- [X] Comment Likes
 	- [X] Repository Comment Likes
 	- [X] Pull Request Comment Likes
- [X] Core
 	- [X] Admin
  		- [X] Groups
  		- [X] Users
  		- [X] Cluster
  		- [X] License
  		- [X] Mail Server
  		- [X] Permissions
  		- [X] Pull Requests
 	- [X] Application Properties
 	- [X] Dashboard
 	- [X] Groups
 	- [X] Hooks
 	- [X] Inbox
 	- [X] Logs
 	- [X] Markup
 	- [X] Profile
 	- [X] Projects
  		- [X] Projects
  		- [X] Permissions
  		- [X] Repos
   			- [X] Repos
   			- [X] Branches
   			- [X] Browse
   			- [X] Changes
   			- [X] Commits
   			- [X] Compare
   			- [X] Diff
   			- [X] Files
   			- [X] Last Modified
   			- [X] Participants
   			- [X] Permissions
   			- [X] Pull Requests
   			- [X] Raw
   			- [X] Settings
   			- [X] Tags
   			- [X] Webhooks
  		- [X] Settings
 	- [X] Repos
 	- [X] Tasks
 	- [X] Users
- [X] Default Reviewers
 	- [X] Project Default Reviewers
 	- [X] Repository Default Reviewers
- [X] Git
- [X] JIRA
 	- [X] Create JIRA Issue
 	- [X] Get Commits For JIRA Issue
 	- [X] Get JIRA Issues For Commits
- [X] Personal Access Tokens
- [X] Ref Restrictions
 	- [X] Project Restrictions
 	- [X] Repository Restrictions
- [X] Repository Ref Synchronization
- [X] SSH
