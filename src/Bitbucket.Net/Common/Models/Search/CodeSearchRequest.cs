namespace Bitbucket.Net.Common.Models.Search;

/// <summary>
/// Request body for the Bitbucket Server code search API.
/// POST /rest/search/latest/search
/// </summary>
public class CodeSearchRequest
{
    /// <summary>
    /// The search query string. Supports Bitbucket search syntax:
    /// repo:slug, project:KEY, lang:, ext:, path:
    /// </summary>
    public required string Query { get; set; }

    /// <summary>
    /// Entity types to search. Use <see cref="SearchEntities.CodeOnly"/> for code search.
    /// </summary>
    public required SearchEntities Entities { get; set; }

    /// <summary>
    /// Pagination limits for the search results.
    /// </summary>
    public required SearchLimits Limits { get; set; }
}

/// <summary>
/// Specifies which entity types to search for.
/// </summary>
public class SearchEntities
{
    /// <summary>
    /// Include code results. Set to an empty object to enable code search.
    /// </summary>
    public SearchEntityFilter? Code { get; set; }

    /// <summary>
    /// Creates entities for a code-only search.
    /// </summary>
    public static SearchEntities CodeOnly => new() { Code = new SearchEntityFilter() };
}

/// <summary>
/// Marker class representing an entity filter in search requests.
/// Serializes to an empty JSON object <c>{}</c>.
/// </summary>
public class SearchEntityFilter { }

/// <summary>
/// Pagination limits for search results.
/// </summary>
public class SearchLimits
{
    /// <summary>
    /// Maximum number of primary results to return. Default: 25.
    /// </summary>
    public int Primary { get; set; } = 25;

    /// <summary>
    /// Maximum number of secondary results per primary result. Default: 10.
    /// </summary>
    public int Secondary { get; set; } = 10;
}