![Icon](https://i.imgur.com/OsDAzyV.png)
# Bitbucket.Net

[![NuGet](https://img.shields.io/nuget/v/BitbucketServer.Net.svg)](https://www.nuget.org/packages/BitbucketServer.Net)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BitbucketServer.Net.svg)](https://www.nuget.org/packages/BitbucketServer.Net)
[![CI](https://github.com/diomonogatari/Bitbucket.Net/actions/workflows/ci.yml/badge.svg)](https://github.com/diomonogatari/Bitbucket.Net/actions/workflows/ci.yml)
[![license](https://img.shields.io/github/license/diomonogatari/Bitbucket.Net.svg?maxAge=2592000)](https://github.com/diomonogatari/Bitbucket.Net/blob/main/LICENSE)
![](https://img.shields.io/badge/.net-10.0-yellowgreen.svg)
![](https://img.shields.io/badge/status-beta-orange.svg)

Modernized C# client for **Bitbucket Server** (Stash) REST API.

## Contributing

Development setup (including the pre-commit formatting hook) is documented in
[CONTRIBUTING.md](CONTRIBUTING.md).

> **Fork notice** &mdash; This is an actively maintained fork of
> [lvermeulen/Bitbucket.Net](https://github.com/lvermeulen/Bitbucket.Net),
> which appears to be abandoned (last release 2020).
> The fork is **not production-ready** yet &mdash; it works well for the
> author's own use case (an MCP Server for on-prem Bitbucket Server) but
> not every endpoint has been fully tested.
> Contributions, bug reports, and feedback are very welcome.

### What changed from the original

- .NET 10 target (dropped .NET Framework / .NET Standard)
- `System.Text.Json` instead of Newtonsoft.Json (2-3x faster, no CVEs)
- `CancellationToken` on every async method
- `IAsyncEnumerable` streaming for paginated endpoints
- Streaming diffs and raw file content
- Typed exception hierarchy (`BitbucketNotFoundException`, etc.)
- `IHttpClientFactory` / DI-friendly constructors
- Bitbucket Server 9.0+ blocker-comment (task) support with legacy fallback
- Flurl.Http 4.x

If you're looking for Bitbucket Cloud API, try [this repository](https://github.com/lvermeulen/Bitbucket.Cloud.Net).

## Installation

```bash
dotnet add package BitbucketServer.Net --prerelease
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

### Dependency Injection with IHttpClientFactory

For production scenarios, you can inject an externally managed `HttpClient` to leverage `IHttpClientFactory` for connection pooling, resilience policies (via Polly), and centralized configuration:

```csharp
// In Program.cs or Startup.cs
services.AddHttpClient<BitbucketClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(2);
})
.AddTransientHttpErrorPolicy(p => 
    p.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
.AddTransientHttpErrorPolicy(p => 
    p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// Register BitbucketClient
services.AddSingleton<BitbucketClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient(nameof(BitbucketClient));
    
    return new BitbucketClient(
        httpClient, 
        "https://bitbucket.example.com",
        () => sp.GetRequiredService<ITokenProvider>().GetToken());
});
```

### Advanced: Using IFlurlClient

For fine-grained control over Flurl's configuration:

```csharp
services.AddSingleton<IFlurlClientCache>(sp => new FlurlClientCache()
    .Add("Bitbucket", "https://bitbucket.example.com", builder => builder
        .WithSettings(s => s.Timeout = TimeSpan.FromMinutes(5))
        .WithHeader("X-Custom-Header", "value")));

services.AddSingleton<BitbucketClient>(sp =>
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
```

### Exception Handling

The library provides typed exceptions for precise error handling:

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
	* [X] Project Events
	* [X] Repository Events
* [X] Branches
	* [X] Create Branch
	* [X] Delete Branch
	* [X] Branch Info
	* [X] Branch Model
* [X] Builds
	* [X] Commits Build Stats
	* [X] Commit Build Stats
	* [X] Commit Build Status
	* [X] Associate Build Status
* [X] Comment Likes
	* [X] Repository Comment Likes
	* [X] Pull Request Comment Likes
* [X] Core
	* [X] Admin
		* [X] Groups
		* [X] Users
		* [X] Cluster
		* [X] License
		* [X] Mail Server
		* [X] Permissions
		* [X] Pull Requests
	* [X] Application Properties
	* [X] Dashboard
	* [X] Groups
	* [X] Hooks
	* [X] Inbox
	* [X] Logs
	* [X] Markup
	* [X] Profile
	* [X] Projects
		* [X] Projects
		* [X] Permissions
		* [X] Repos
			* [X] Repos
			* [X] Branches
			* [X] Browse
			* [X] Changes
			* [X] Commits
			* [X] Compare
			* [X] Diff
			* [X] Files
			* [X] Last Modified
			* [X] Participants
			* [X] Permissions
			* [X] Pull Requests
			* [X] Raw
			* [X] Settings
			* [X] Tags
			* [X] Webhooks
		* [X] Settings
	* [X] Repos
	* [X] Tasks
	* [X] Users
* [X] Default Reviewers
	* [X] Project Default Reviewers
	* [X] Repository Default Reviewers
* [X] Git
* [X] JIRA
	* [X] Create JIRA Issue
	* [X] Get Commits For JIRA Issue
	* [X] Get JIRA Issues For Commits
* [X] Personal Access Tokens
* [X] Ref Restrictions
	* [X] Project Restrictions
	* [X] Repository Restrictions
* [X] Repository Ref Synchronization
* [X] SSH
