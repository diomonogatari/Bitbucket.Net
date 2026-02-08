namespace Bitbucket.Net.Common.Models;

/// <summary>
/// Represents a Bitbucket paged response containing a collection of items.
/// </summary>
public class PagedResults<T> : PagedResultsBase
{
    /// <summary>
    /// Gets or sets the maximum number of items returned in this page.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the page of items returned by the request.
    /// </summary>
    public List<T> Values { get; set; } = [];

    /// <summary>
    /// Gets or sets the starting offset of the next page, when available.
    /// </summary>
    public int? NextPageStart { get; set; }

    /// <summary>
    /// MCP-friendly property indicating if more results are available.
    /// Per MCP best practices, pagination responses should include has_more.
    /// </summary>
    public bool HasMore => !IsLastPage;

    /// <summary>
    /// MCP-friendly property for the current offset position.
    /// Per MCP best practices, pagination responses should include current offset.
    /// </summary>
    public int CurrentOffset => Start;
}