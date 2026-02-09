using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Models.Search;

/// <summary>
/// Top-level response from the Bitbucket Server code search API.
/// </summary>
public class CodeSearchResponse
{
    /// <summary>
    /// The scope of the search (e.g., GLOBAL).
    /// </summary>
    public SearchScope? Scope { get; set; }

    /// <summary>
    /// Code search results.
    /// </summary>
    public CodeSearchCategory? Code { get; set; }

    /// <summary>
    /// Query metadata including whether query substitution occurred.
    /// </summary>
    public SearchQuery? Query { get; set; }
}

/// <summary>
/// Scope metadata for a search.
/// </summary>
public class SearchScope
{
    /// <summary>
    /// The scope type, e.g., "GLOBAL".
    /// </summary>
    public string? Type { get; set; }
}

/// <summary>
/// Query metadata.
/// </summary>
public class SearchQuery
{
    /// <summary>
    /// Whether the query was substituted (e.g., spell correction).
    /// </summary>
    public bool Substituted { get; set; }
}

/// <summary>
/// Category of code search results with pagination info.
/// </summary>
public class CodeSearchCategory
{
    /// <summary>
    /// Result category name (e.g., "primary").
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Whether this is the last page of results.
    /// </summary>
    public bool IsLastPage { get; set; }

    /// <summary>
    /// Total number of results matching the query.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Starting index of the current page.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Starting index for the next page, if available.
    /// </summary>
    public int? NextStart { get; set; }

    /// <summary>
    /// The code search result items.
    /// </summary>
    public List<CodeSearchResult> Values { get; set; } = [];
}

/// <summary>
/// A single code search result representing a file with matching content.
/// </summary>
public class CodeSearchResult
{
    /// <summary>
    /// The repository containing the matching file.
    /// </summary>
    public Repository? Repository { get; set; }

    /// <summary>
    /// The file path within the repository.
    /// </summary>
    public string? File { get; set; }

    /// <summary>
    /// Groups of matching lines with surrounding context.
    /// Each inner list represents a contiguous block of context lines.
    /// </summary>
    public List<List<CodeSearchHitLine>>? HitContexts { get; set; }

    /// <summary>
    /// Segments of the file path that matched the query.
    /// </summary>
    public List<SearchPathMatch>? PathMatches { get; set; }

    /// <summary>
    /// Total number of hits in this file.
    /// </summary>
    public int HitCount { get; set; }
}

/// <summary>
/// A single line in a code search hit context block.
/// </summary>
public class CodeSearchHitLine
{
    /// <summary>
    /// The 1-based line number.
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// The line text content. May contain &lt;em&gt; tags highlighting matched terms.
    /// </summary>
    public string? Text { get; set; }
}

/// <summary>
/// Represents a matching segment in the file path.
/// </summary>
public class SearchPathMatch
{
    /// <summary>
    /// Starting character index of the match in the path.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Length of the matching text.
    /// </summary>
    public int Length { get; set; }
}