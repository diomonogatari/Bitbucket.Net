using System.Collections.Generic;

namespace Bitbucket.Net.Common.Models;

public class PagedResults<T> : PagedResultsBase
{
    public int Limit { get; set; }
    public List<T> Values { get; set; } = [];
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